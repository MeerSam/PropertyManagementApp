import { HttpClient, HttpParams } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { environment } from '../../environments/environment';
import { Document, DocumentPresignedUrl } from '../../types/document';

@Injectable({
  providedIn: 'root',
})
export class DocumentService {
  private http = inject(HttpClient);
  private baseUrl = environment.apiUrl;

  getCommunityDocuments() {
    return this.http.get<Document[]>(this.baseUrl + 'documents/community')
  }

  getPresignedUrl(id: string, mode: string = 'preview') {
    const params = new HttpParams().set('mode', mode);
    return this.http.get<DocumentPresignedUrl>(this.baseUrl + 'documents/' + id + '/url', { params })
  } 
}
