import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { ContactService } from '../../services/contact';
import { Contact } from '../../models/contact';

@Component({
  selector: 'app-contact-detail',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink],
  templateUrl: './contact-detail.html',
  styleUrl: './contact-detail.css'
})
export class ContactDetail implements OnInit {
  contact = signal<Contact | null>(null);
  loading = signal(true);
  error = signal('');
  deleting = signal(false);
  detailErrors = signal<string[]>([]);
  savingDetails = signal(false);

  newPhoneType = 'Mobile';
  newPhoneValue = '';
  newEmailType = 'Personal';
  newEmailValue = '';
  newTag = '';

  private contactId!: number;

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private contactService: ContactService
  ) {}

  ngOnInit(): void {
    const idParam = this.route.snapshot.paramMap.get('id');
    const id = idParam ? Number(idParam) : null;

    if (id === null || isNaN(id)) {
      this.error.set('Nevažeći ID kontakta.');
      this.loading.set(false);
      return;
    }

    this.contactId = id;
    this.loadContact();
  }

  private loadContact(): void {
    this.loading.set(true);
    this.contactService.getContactById(this.contactId).subscribe({
      next: (result) => {
        this.contact.set(result);
        this.loading.set(false);
      },
      error: (err) => {
        this.error.set(err.status === 404 ? 'Kontakt nije pronađen.' : 'Greška pri dohvaćanju kontakta: ' + err.message);
        this.loading.set(false);
      }
    });
  }

  onDelete(): void {
    const current = this.contact();
    if (!current) return;

    const confirmed = window.confirm(`Jesi li siguran da želiš obrisati kontakt ${current.firstName} ${current.lastName}?`);
    if (!confirmed) return;

    this.deleting.set(true);
    this.contactService.deleteContact(current.id).subscribe({
      next: () => this.router.navigate(['/contacts']),
      error: () => {
        this.deleting.set(false);
        this.error.set('Brisanje nije uspjelo. Pokušajte ponovno.');
      }
    });
  }

  addPhone(): void {
    if (!this.newPhoneValue.trim()) return;

    this.savingDetails.set(true);
    this.detailErrors.set([]);

    this.contactService.addContactDetails(this.contactId, {
      phoneNumbers: [{ type: this.newPhoneType, value: this.newPhoneValue }],
      emails: [],
      tags: []
    }).subscribe({
      next: (updated) => {
        this.contact.set(updated);
        this.newPhoneValue = '';
        this.savingDetails.set(false);
      },
      error: (err) => this.handleDetailError(err)
    });
  }

  removePhone(phoneId: number): void {
    this.savingDetails.set(true);
    this.contactService.removeContactDetails(this.contactId, {
      phoneNumberIds: [phoneId],
      emailIds: [],
      tagIds: []
    }).subscribe({
      next: (updated) => {
        this.contact.set(updated);
        this.savingDetails.set(false);
      },
      error: (err) => this.handleDetailError(err)
    });
  }

  addEmail(): void {
    if (!this.newEmailValue.trim()) return;

    this.savingDetails.set(true);
    this.detailErrors.set([]);

    this.contactService.addContactDetails(this.contactId, {
      phoneNumbers: [],
      emails: [{ type: this.newEmailType, value: this.newEmailValue }],
      tags: []
    }).subscribe({
      next: (updated) => {
        this.contact.set(updated);
        this.newEmailValue = '';
        this.savingDetails.set(false);
      },
      error: (err) => this.handleDetailError(err)
    });
  }

  removeEmail(emailId: number): void {
    this.savingDetails.set(true);
    this.contactService.removeContactDetails(this.contactId, {
      phoneNumberIds: [],
      emailIds: [emailId],
      tagIds: []
    }).subscribe({
      next: (updated) => {
        this.contact.set(updated);
        this.savingDetails.set(false);
      },
      error: (err) => this.handleDetailError(err)
    });
  }

  addTagToContact(): void {
    if (!this.newTag.trim()) return;

    this.savingDetails.set(true);
    this.detailErrors.set([]);

    this.contactService.addContactDetails(this.contactId, {
      phoneNumbers: [],
      emails: [],
      tags: [this.newTag]
    }).subscribe({
      next: (updated) => {
        this.contact.set(updated);
        this.newTag = '';
        this.savingDetails.set(false);
      },
      error: (err) => this.handleDetailError(err)
    });
  }

  removeTagFromContact(tagId: number): void {
    this.savingDetails.set(true);
    this.contactService.removeContactDetails(this.contactId, {
      phoneNumberIds: [],
      emailIds: [],
      tagIds: [tagId]
    }).subscribe({
      next: (updated) => {
        this.contact.set(updated);
        this.savingDetails.set(false);
      },
      error: (err) => this.handleDetailError(err)
    });
  }

  private handleDetailError(err: any): void {
    this.savingDetails.set(false);
    if (err.status === 400 && Array.isArray(err.error)) {
      this.detailErrors.set(err.error);
    } else if (err.status === 404) {
      this.detailErrors.set(['Kontakt nije pronađen.']);
    } else {
      this.detailErrors.set(['Došlo je do greške. Pokušajte ponovno.']);
    }
  }
}