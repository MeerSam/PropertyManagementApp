import { Component, computed, effect, ElementRef, inject, Input, input, OnInit, signal, ViewChild } from '@angular/core';
import { DocumentService } from '../../../core/services/document-service';
import { SessionService } from '../../../core/services/session-service';
import { Document, DocumentScope, DocumentParams, DOCUMENT_SCOPES } from '../../../types/document';
import { DatePipe } from '@angular/common';
import { SafeUrlPipe } from "../../../core/pipes/safe-url-pipe";
import { DocumentUpload } from "../../../shared/document-upload/document-upload";
import { ToastService } from '../../../core/services/toast-service';
import { DeleteButton } from "../../../shared/delete-button/delete-button";
import { Property, PropertyOwnership } from '../../../types/property';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-document-records',
  imports: [DatePipe, SafeUrlPipe, DeleteButton, DocumentUpload],
  templateUrl: './document-records.html',
  styleUrl: './document-records.css',
})
export class DocumentRecords implements OnInit {

  // we need to access the modal inside our component. @ViewChild to access it
  @ViewChild('docPreviewModal') docPreviewModal!: ElementRef<HTMLDialogElement>;
  @ViewChild('docUploadModal') docUploadModal!: ElementRef<HTMLDialogElement>;

  protected documentService = inject(DocumentService);
  protected toast = inject(ToastService);
  protected session = inject(SessionService);
  private route = inject(ActivatedRoute);

  memberId = input<string | null>(null);
  scope = input<DocumentScope | null>(null);
  scopeRoute = signal<DocumentScope | null>(null);
  propertyId = input<string | null>(null);
  ownershipId = input<string | null>(null);
  ownerships = input<PropertyOwnership[]>([]);
  property= input<Property | null>(null);
 

  private validScopes: DocumentScope[] = [
    'Public',
    'Community',
    'PropertyHistory',
    'OwnerTenure'
  ];


  protected adminRoles = ['admin', 'board_member', 'property_manager'];
  protected loading = signal(false);
  protected documents = signal<Document[]>([]);
  protected selectedDocument = signal<Document | null>(null);

  protected resolvedScope = computed<DocumentScope | null>(() => {
    let scope: DocumentScope | null;

    if (this.scopeRoute()) {
      scope = this.scopeRoute();
    } else {
      scope = this.scope();
    }

    if (scope === undefined) {
      return null
    }
    return scope;
  });


  protected resolvedParams = computed<DocumentParams>(() => {
    if (this.memberId()) {
      return {
        scope: this.resolvedScope(),
        memberId: this.memberId() ?? null,
        propertyId: this.propertyId() ?? null,
        propertyOwnershipId: this.ownershipId() ?? null,
      } as DocumentParams;
    }

    switch (this.resolvedScope()) {
      case 'Public':
      case 'Community':
        // // *meera console.log('I am here, community', { scope: this.resolvedScope() })
        return { scope: this.resolvedScope() } as DocumentParams;
      case 'PropertyHistory':
        // // *meera console.log('I am here, PropertyHistory', { scope: this.resolvedScope() })
        return {
          scope: this.resolvedScope(),
          propertyId: this.propertyId() ?? null,
          memberId: this.memberId()
        } as DocumentParams;
      case 'OwnerTenure':
        // // *meera console.log('I am here, OwnerTenure', { scope: this.resolvedScope() })
        return {
          scope: this.resolvedScope(),
          propertyOwnershipId: this.ownershipId(),
          memberId: this.memberId() ?? null
        } as DocumentParams;

      default:
        return { scope: 'Community' } as DocumentParams;;
    }

  });

  private toDocumentScope(value: any): DocumentScope | null {
    if (typeof value !== 'string') return null;
    return (DOCUMENT_SCOPES as readonly string[]).includes(value.trim())
      ? (value.trim() as DocumentScope)
      : null;
  }

  // Validate params before loading
  protected isParamsValid = computed<boolean>(() => {
    const params = this.resolvedParams();
    // MEMBER-BASED LOADING
    if (params?.memberId) {
      return true;
    }


    // SCOPE-BASED LOADING
    switch (params?.scope) {
      case 'Community':
      case 'Public':
        // // *meera console.log('I am here, community  valid param',{ scope: this.resolvedScope(), paramScope: params?.scope })
        return true;

      case 'PropertyHistory':
        // console.log('I am here, PropertyHistory valid param',
        //   {
        //     scope: this.resolvedScope(),
        //     paramScope: params?.scope,
        //     propertyId: !!params?.propertyId || !!params.memberId,
        //     memberId: !!params.memberId,
        //     retruned: !!params?.propertyId,
        //     params: params
        //   })
        // console.log(this.resolvedParams());
        return !!params?.propertyId || !!params.memberId;

      case 'OwnerTenure':
        // console.log('I am here, OwnerTenure valid param', { scope: this.resolvedScope(), paramScope: params?.scope, retruned: !!params?.propertyId && !!params.memberId })

        return !!params?.propertyOwnershipId || !!params.memberId;

      default:
        // // *meera console.log('I am here, default valid param', { scope: this.resolvedScope(), paramScope: params?.scope })

        return false; // scope missing or invalid
    }
  });


  ngOnInit(): void {
    this.route.parent?.data.subscribe(data => {
      if (data['scope'] as DocumentScope) {
        this.scopeRoute.set(data['scope']);
      };
    });


    this.loadDocuments();

  }

  constructor() {
    // This effect runs every time input changes
    effect(() => {
      this.loadDocuments();
    });
  }
  canAddDocument(scope: DocumentScope | undefined | null): boolean {
    const role = this.session.currentRole();
    if (this.adminRoles.includes(role ?? '')) return true;

    if (this.memberId() && scope == "OwnerTenure" && this.ownershipId()) {
      // Member-based rules 
      return this.adminRoles.includes(role ?? '') ||
        this.session.activeClient()?.memberId === this.memberId();
    }

    switch (this.resolvedScope()) {
      case "Public":
      case "Community":
        return this.adminRoles.includes(role ?? '');

      case "PropertyHistory":
        return !!this.propertyId() && (this.adminRoles.includes(role ?? '') ||
          this.session.activeClient()?.memberId === this.memberId())
      case "OwnerTenure":
        return !!this.ownershipId() && (this.adminRoles.includes(role ?? '') ||
          this.session.activeClient()?.memberId === this.memberId())

      default:
        return (this.adminRoles.includes(role ?? '')) ? true : false
    }
  }



  loadDocuments() {
    // console.log('Loading document... ', this.resolvedParams())
    const params = this.resolvedParams() ?? { scope: 'Community' } as DocumentParams;
    // console.log('(params)', params)
    if (!this.isParamsValid()) {
      this.toast.error(this.getMissingParamsError());
      return
    }
    // console.log('(params)', params)
    this.documentService.getDocuments(params).subscribe({
      next: docs => {
        // console.log('this.resolvedParams().scope', this.resolvedParams().scope)
        const  filtered = docs.filter(d =>
          !this.resolvedParams().scope ||
          d.scope === this.resolvedParams().scope
        )
        // console.log('returned docs', filtered)
        this.documents.set(filtered);
        this.loading.set(false)
      },
      error: err => {
        // console.log(console.error(err));
        this.toast.error('Failed to load documents.');
        this.loading.set(false);
      }
    });
  }

  openPreviewModal(doc: Document) {
    this.selectedDocument.set(doc);
    this.docPreviewModal.nativeElement.showModal();
  }
  closePreviewModal() {
    this.selectedDocument.set(null);
    this.docPreviewModal.nativeElement.close();
  }

  onRowClickAction(selectedDoc: Document, mode: string = 'preview') {
    if (selectedDoc.presignedUrl?.mode === mode) {
      // URL already exists and matches — use it directly
      this._handleAction(selectedDoc, mode)
    } else {
      // Fetch first, then act inside the subscription callback
      this.requestPresignedUrl(selectedDoc, mode)
    }
  }
  requestPresignedUrl(doc: Document, mode: string = 'preview') {
    this.documentService.getPresignedUrl(doc.id, mode)
      .subscribe(urlInfo => {
        // Update the row object
        const updated = { ...doc, presignedUrl: urlInfo };
        // Update the documents signal
        this.documents.update(list =>
          list.map(d => d.id === doc.id ? updated : d)
        );
        this._handleAction(updated, mode);
      });
  }

  deleteDocument(documentId: string) {
    // // *meera console.log(`Deleting ID ${documentId}`)
    this.documents.update(list =>
      list.filter(d => d.id !== documentId)
    );
  }
  closeUploadModal() {
    this.loading.set(false)
    this.docUploadModal.nativeElement.close()
  }

  openUploadModal() {
    this.loading.set(true);
    this.docUploadModal.nativeElement.showModal();
    // // *meera console.log("openUploadModal");
  }

  updateDocumentList(document: Document) {
    if (document) {
      this.documents.update(list =>
        [...list, document]
      );
      this.toast.success(`New Document created...${document.title}`)
    }
    this.closeUploadModal();
  }



  private getMissingParamsError(): string {
    const params = this.resolvedParams();
    // // *meera console.log('I am here, getMissingParamsError', { scope: this.resolvedScope() })
    // //*meera console.log(.log(this.resolvedParams())

    // Scope is always required when memberId is missing
    if (!params?.memberId && !params?.scope) {
      return 'Document scope is required.';
    }

    // If memberId exists → always valid, no missing params
    if (params?.memberId) {
      return '';
    }

    // SCOPE-BASED VALIDATION
    const missing: string[] = [];

    switch (params.scope) {
      case 'Community':
      case 'Public':
        // No extra params required
        return '';

      case 'PropertyHistory':
        if (!params.propertyId) missing.push('Property ID');
        break;

      case 'OwnerTenure':
        if (!params.propertyOwnershipId) missing.push('Ownership ID');
        break;

      default:
        return 'Document scope is invalid.';
    }

    // If nothing is missing → valid
    if (missing.length === 0) return '';

    // Build readable error message
    return `${params.scope} documents require: ${missing.join(' and ')}.`;
  }

  private _handleAction(doc: Document, mode: string) {
    if (mode === 'download') {
      this._triggerDownload(doc);
    } else {
      this.openPreviewModal(doc);
    }
  }
  private _triggerDownload(doc: Document) {
    if (!doc.presignedUrl?.url) return;
    const anchor = document.createElement('a');
    anchor.href = doc.presignedUrl.url;
    anchor.download = doc.fileName;
    anchor.style.display = 'none';
    document.body.appendChild(anchor);
    anchor.click();
    setTimeout(() => document.body.removeChild(anchor), 100);
  }
}
