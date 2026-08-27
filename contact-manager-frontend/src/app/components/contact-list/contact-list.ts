import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { ContactService } from '../../services/contact';
import { Contact } from '../../models/contact';

@Component({
  selector: 'app-contact-list',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink],
  templateUrl: './contact-list.html',
  styleUrl: './contact-list.css'
})
export class ContactList implements OnInit {
  contacts = signal<Contact[]>([]);
  loading = signal(true);
  error = signal('');

  totalCount = signal(0);
  totalPages = signal(0);
  page = signal(1);
  pageSize = 10;

  searchTerm = '';
  tagFilter = '';
  sortBy = 'lastName';

  constructor(private contactService: ContactService) {}

  ngOnInit(): void {
    this.load();
  }

  load(): void {
    this.loading.set(true);
    this.error.set('');

    this.contactService
      .getContacts(this.searchTerm || undefined, this.tagFilter || undefined, this.sortBy, this.page(), this.pageSize)
      .subscribe({
        next: (result) => {
          this.contacts.set(result.items);
          this.totalCount.set(result.totalCount);
          this.totalPages.set(result.totalPages);
          this.loading.set(false);
        },
        error: (err) => {
          this.error.set('Greška pri dohvaćanju kontakata: ' + err.message);
          this.loading.set(false);
        }
      });
  }

  onSearchSubmit(): void {
    this.page.set(1); 
    this.load();
  }

  onSortChange(): void {
    this.page.set(1);
    this.load();
  }

  goToPage(newPage: number): void {
    if (newPage < 1 || newPage > this.totalPages()) return;
    this.page.set(newPage);
    this.load();
  }

  clearFilters(): void {
    this.searchTerm = '';
    this.tagFilter = '';
    this.sortBy = 'lastName';
    this.page.set(1);
    this.load();
  }
}