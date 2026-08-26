import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { ContactService } from '../../services/contact';
import { Contact } from '../../models/contact';

@Component({
  selector: 'app-contact-detail',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './contact-detail.html',
  styleUrl: './contact-detail.css'
})
export class ContactDetail implements OnInit {
  contact = signal<Contact | null>(null);
  loading = signal(true);
  error = signal('');
  deleting = signal(false);

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

    this.contactService.getContactById(id).subscribe({
      next: (result) => {
        this.contact.set(result);
        this.loading.set(false);
      },
      error: (err) => {
        if (err.status === 404) {
          this.error.set('Kontakt nije pronađen.');
        } else {
          this.error.set('Greška pri dohvaćanju kontakta: ' + err.message);
        }
        this.loading.set(false);
      }
    });
  }

  onDelete(): void {
    const current = this.contact();
    if (!current) return;

    const confirmed = window.confirm(
      `Jesi li siguran da želiš obrisati kontakt ${current.firstName} ${current.lastName}?`
    );
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
}