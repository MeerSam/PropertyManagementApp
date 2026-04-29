import { Component, ElementRef, inject, OnInit, signal, ViewChild } from '@angular/core';
import { DocumentService } from '../../../core/services/document-service';
import { SessionService } from '../../../core/services/session-service';
import { Document, DocumentUploadRequest, DocumentCategory, DocumentScope } from '../../../types/document';
import { DatePipe } from '@angular/common';
import { SafeUrlPipe } from "../../../core/pipes/safe-url-pipe";
import { FileUpload } from '../../../shared/file-upload/file-upload';
import { UploadService } from '../../../core/services/upload-service';
import { DocumentUpload } from "../../../shared/document-upload/document-upload";
import { ToastService } from '../../../core/services/toast-service';

@Component({
  selector: 'app-document-list',
  imports: [DatePipe, SafeUrlPipe, DocumentUpload],
  templateUrl: './document-list.html',
  styleUrl: './document-list.css',
})
export class DocumentList implements OnInit {

  // we need to access the modal inside our component. @ViewChild to access it
  @ViewChild('docPreviewModal') docPreviewModal!: ElementRef<HTMLDialogElement>;
  @ViewChild('docUploadModal') docUploadModal!: ElementRef<HTMLDialogElement>;  
  
  protected documentService = inject(DocumentService);
  protected toast = inject(ToastService);

  protected documents = signal<Document[]>([]);
  protected selectedDocument = signal<Document | null>(null);
  protected loading = signal(false);

  ngOnInit(): void {
    this.loadDocuments()
  }

  loadDocuments() {
    this.documentService.getCommunityDocuments().subscribe({
      next: docs => {
        this.documents.set(docs);
      }
    })
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
    console.log("openUploadModal"); 
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




}