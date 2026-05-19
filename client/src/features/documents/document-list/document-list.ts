import { Component, computed, ElementRef, inject, input, OnInit, signal, ViewChild } from '@angular/core';
import { DocumentService } from '../../../core/services/document-service';
import { SessionService } from '../../../core/services/session-service';
import { Document, DocumentScope, DocumentParams } from '../../../types/document';
import { DatePipe } from '@angular/common';
import { SafeUrlPipe } from "../../../core/pipes/safe-url-pipe";
import { DocumentUpload } from "../../../shared/document-upload/document-upload";
import { ToastService } from '../../../core/services/toast-service';
import { DeleteButton } from "../../../shared/delete-button/delete-button";

@Component({
  selector: 'app-document-list',
  imports: [DatePipe, SafeUrlPipe, DocumentUpload, DeleteButton],
  templateUrl: './document-list.html',
  styleUrl: './document-list.css',
})
export class DocumentList {

  // we need to access the modal inside our component. @ViewChild to access it
  @ViewChild('docPreviewModal') docPreviewModal!: ElementRef<HTMLDialogElement>;
  @ViewChild('docUploadModal') docUploadModal!: ElementRef<HTMLDialogElement>;

  protected documentService = inject(DocumentService);
  protected toast = inject(ToastService);
  protected session = inject(SessionService);

  documentParams = input<DocumentParams>();

  protected documents = signal<Document[]>([]);
  protected selectedDocument = signal<Document | null>(null);
  protected loading = signal(false);
  protected adminRoles = ['admin', 'board_member', 'property_manager'];

  // Scopes that require propertyId + ownershipId
  private readonly scopesRequiringParams: DocumentScope[] = ['PropertyHistory', 'OwnerTenure'];

  private resolvedParams = computed<DocumentParams>(() =>
    this.documentParams() ?? { scope: 'Community' } as DocumentParams
  );


  // Validate params before loading
  protected isParamsValid = computed(() => {
    const params = this.resolvedParams();
    // undefined scope means == getAllDocs by Member
    if (!params?.scope &&  !params?.memberId) return false;

    if (params?.scope && this.scopesRequiringParams.includes(params.scope)) {
      // These scopes MUST have both IDs

       
       
      return (params?.scope === 'PropertyHistory' && (!!params?.propertyId || !!params?.memberId)) ||
        (params?.scope === 'OwnerTenure' &&   
          (!!params?.propertyOwnershipId || !!params?.memberId ||
            (params?.ownerships && params?.ownerships?.length>0))) ;
    }

    // Public and Community need no extra params
    return true;
  });

  constructor()  {
    this.loadDocuments()
  }

  loadDocuments() {
    const params = this.resolvedParams() ?? { scope: 'Community' } as DocumentParams;

    if (!this.isParamsValid()) {
      this.toast.error(this.getMissingParamsError());
    }
    this.documentService.getDocuments(params).subscribe({
      next: docs => {
        this.documents.set(docs);
        this.loading.set(false)
      },
      error: err => {
// //*meera console.log(.error(err);
        this.toast.error('Failed to load documents.');
        this.loading.set(false);
      }
    });
  }
  closePreviewModal() {
    this.selectedDocument.set(null);
    this.docPreviewModal.nativeElement.close();
  }
  closeUploadModal() {
    this.loading.set(false)
    this.docUploadModal.nativeElement.close()
  }

  openUploadModal() {
    this.loading.set(true);
    this.docUploadModal.nativeElement.showModal();
// //*meera console.log(.log("openUploadModal");
  }

  openPreviewModal(doc: Document) {
    this.selectedDocument.set(doc);
    this.docPreviewModal.nativeElement.showModal();
  }
  onRowClickAction(doc: Document, mode: string = 'preview') {
    if (doc.presignedUrl?.mode === mode) {
      // URL already exists and matches — use it directly
      this._handleAction(doc, mode)
    } else {
      // Fetch first, then act inside the subscription callback
      this.requestPresignedUrl(doc, mode)
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

  updateDocumentList(document: Document) {
    if (document) {
      this.documents.update(list =>
        [...list, document]
      );
      this.toast.success(`New Document created...${document.title}`)
    }
    this.closeUploadModal();
  }

  deleteDocument(documentId: string) {
// //*meera console.log(.log(`Deleting ID ${documentId}`)
    this.documents.update(list =>
      list.filter(d => d.id !== documentId)
    );
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

  private _handleAction(doc: Document, mode: string) {
    if (mode === 'download') {
      this._triggerDownload(doc);
    } else {
      this.openPreviewModal(doc);
    }
  }

  private getMissingParamsError(): string {
    const params = this.resolvedParams();
    if (!params?.scope) return 'Document scope is required.';

    const missing: string[] = [];

    if (!params.memberId ){
      if (!params.propertyId && params?.scope === 'PropertyHistory' ) missing.push('Property ID');
    if (!params.propertyOwnershipId &&  params?.scope === 'OwnerTenure' ) missing.push('Ownership ID');

    } 
    return `${params.scope} documents require: ${missing.join(' and ')}.`;
  }



}