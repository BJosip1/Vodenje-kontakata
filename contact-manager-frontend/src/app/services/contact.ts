import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment.development';
import { Contact } from '../models/contact';
import { PagedResult } from '../models/paged-result';
import { PostContact } from '../models/post-contact';
import { PutContact } from '../models/put-contact';


@Injectable({
  providedIn: 'root'
})
export class ContactService {
  private baseUrl = `${environment.apiUrl}/contact`;

  constructor(private http: HttpClient) {}

  getContacts(search?: string, tag?: string, sortBy?: string, page = 1, pageSize = 20): Observable<PagedResult<Contact>> {
    let params: any = { page, pageSize };
    if (search) params.search = search;
    if (tag) params.tag = tag;
    if (sortBy) params.sortBy = sortBy;

    return this.http.get<PagedResult<Contact>>(this.baseUrl, { params });
  }

  getContactById(id: number): Observable<Contact> {
    return this.http.get<Contact>(`${this.baseUrl}/${id}`);
  }

  createContact(dto: PostContact): Observable<Contact> {
    return this.http.post<Contact>(this.baseUrl, dto);
  }

  updateContact(id: number, dto: PutContact): Observable<Contact> {
    return this.http.put<Contact>(`${this.baseUrl}/${id}`, dto);
  }

  deleteContact(id: number): Observable<void> {
  return this.http.delete<void>(`${this.baseUrl}/${id}`);
}
}