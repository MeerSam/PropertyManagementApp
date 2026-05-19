import { HttpClient, HttpParams } from '@angular/common/http';
import { inject, Injectable, signal } from '@angular/core';
import { environment } from '../../environments/environment';
import { Document, DocumentParams, DocumentPresignedUrl } from '../../types/document';
import { of } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class DocumentService {
  private http = inject(HttpClient);
  private baseUrl = environment.apiUrl;
  editMode = signal(false);

  getCommunityDocuments() {
    return this.http.get<Document[]>(this.baseUrl + 'documents/community')
  }

  getDocuments(params: DocumentParams) {
    if (!!params.memberId) {
      return this.getDocumentsByMember(params);
    } else if (params.scope === 'Community') {
      return this.getCommunityDocuments();
    } else if (params.scope === "PropertyHistory" && params.propertyId) {
      return this.getPropertyDocuments(params.propertyId ?? '');
    } else if (params.scope === "OwnerTenure" && params.propertyOwnershipId) {
      return this.getOwnerDocuments(params.propertyOwnershipId ?? '')
    }
    return of();
  }

  getDocumentsByMember(documentParams: DocumentParams) {
    let params = new HttpParams();

    if (documentParams.memberId) {
      // params = params.append('memberId', documentParams.memberId);
      if (documentParams.propertyId) {
        params = params.append('propertyId', documentParams.propertyId);
      }
      if (documentParams.propertyOwnershipId) {
        params = params.append('propertyOwnershipId', documentParams.propertyOwnershipId);
      }
      // if (documentParams.scope) {
      //   params = params.append('scope', documentParams.scope);
      // }
      console.log(this.baseUrl + 'documents/member/' + documentParams.memberId)
      console.log(params)

      return this.http.get<Document[]>(this.baseUrl + 'documents/member/' + documentParams.memberId, { params })
    }
    return of();
  }

  getPropertyDocuments(propertyId: string) {
    return this.http.get<Document[]>(this.baseUrl + 'documents/property/' + propertyId + '/history')
  }


  getOwnerDocuments(ownershipId: string) {
    return this.http.get<Document[]>(this.baseUrl + 'documents/ownership/' + ownershipId + '/docs')
  }

  getPresignedUrl(id: string, mode: string = 'preview') {
    const params = new HttpParams().set('mode', mode);
    return this.http.get<DocumentPresignedUrl>(this.baseUrl + 'documents/' + id + '/url', { params })
  }
}
