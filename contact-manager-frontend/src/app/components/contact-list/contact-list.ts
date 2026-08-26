import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { ContactService } from '../../services/contact';
import { Contact } from '../../models/contact';

@Component({
  selector: 'app-contact-list',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './contact-list.html',
  styleUrl: './contact-list.css'
})
export class ContactList implements OnInit {
  contacts = signal<Contact[]>([]);
  loading = signal(true);
  error = signal('');

  constructor(private contactService: ContactService) {}

  ngOnInit(): void {
    this.contactService.getContacts().subscribe({
      next: (result) => {
        this.contacts.set(result.items);
        this.loading.set(false);
      },
      error: (err) => {
        this.error.set('Greška pri dohvaćanju kontakata: ' + err.message);
        this.loading.set(false);
      }
    });
  }
}