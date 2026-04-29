import { inject, Injectable } from '@angular/core';
import { DocumentService } from './document-service';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../environments/environment';
import { Document, DocumentUploadRequest } from '../../types/document';

@Injectable({
  providedIn: 'root',
})
export class UploadService {
  private http =inject(HttpClient);
  protected documentService = inject(DocumentService);
  private baseUrl = environment.apiUrl

  uploadDocument(file: File, documentInfo: DocumentUploadRequest ){
    const formData = new FormData();
    formData.append('title', documentInfo.title);
    formData.append('description', documentInfo.description ?? '');
    formData.append('scope', documentInfo.scope);
    formData.append('category', documentInfo.category);
    formData.append('propertyId', documentInfo.propertyId ?? '');
    formData.append('propertyOwnershipId', documentInfo.propertyOwnershipId ?? '');
    formData.append('notes', documentInfo.notes ?? '');
    formData.append('supersededDocumentId', documentInfo.supersededDocumentId ?? '');


    formData.append('file', file);

    return this.http.post<Document>(this.baseUrl+ 'documents/upload', formData)
  }
  
}
