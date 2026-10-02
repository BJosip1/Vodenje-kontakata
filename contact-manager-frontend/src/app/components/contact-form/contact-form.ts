import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, FormArray, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { ContactService } from '../../services/contact';

@Component({
  selector: 'app-contact-form',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterLink],
  templateUrl: './contact-form.html',
  styleUrl: './contact-form.css'
})
export class ContactForm implements OnInit {
  form!: FormGroup;
  isEditMode = signal(false);
  contactId: number | null = null;
  loading = signal(false);
  errors = signal<string[]>([]);

  constructor(
    private fb: FormBuilder,
    private route: ActivatedRoute,
    private router: Router,
    private contactService: ContactService
  ) {}

  ngOnInit(): void {
    this.form = this.fb.group({
      firstName: ['', Validators.required],
      lastName: ['', Validators.required],
      note: [''],
      address: this.fb.group({
        street: ['', Validators.required],
        city: ['', Validators.required],
        postalCode: ['', Validators.required],
        country: ['', Validators.required]
      }),
      phoneNumbers: this.fb.array([]),
      emails: this.fb.array([]),
      tags: this.fb.array([])
    });

    const idParam = this.route.snapshot.paramMap.get('id');
    if (idParam) {
      this.isEditMode.set(true);
      this.contactId = Number(idParam);
      this.loading.set(true);

      this.contactService.getContactById(this.contactId).subscribe({
        next: (contact) => {
          this.form.patchValue({
            firstName: contact.firstName,
            lastName: contact.lastName,
            note: contact.note,
            address: contact.address
          });
          this.loading.set(false);
        },
        error: () => {
          this.errors.set(['Kontakt nije pronađen.']);
          this.loading.set(false);
        }
      });
    }
  }

  get phoneNumbers(): FormArray {
    return this.form.get('phoneNumbers') as FormArray;
  }

  get emails(): FormArray {
    return this.form.get('emails') as FormArray;
  }

  get tags(): FormArray {
    return this.form.get('tags') as FormArray;
  }

  addPhoneNumber(): void {
    this.phoneNumbers.push(this.fb.group({
      type: ['Mobile', Validators.required],
      value: ['', Validators.required]
    }));
  }

  removePhoneNumber(index: number): void {
    this.phoneNumbers.removeAt(index);
  }

  addEmail(): void {
    this.emails.push(this.fb.group({
      type: ['Personal', Validators.required],
      value: ['', Validators.required]
    }));
  }

  removeEmail(index: number): void {
    this.emails.removeAt(index);
  }

  addTag(): void {
    this.tags.push(this.fb.control('', Validators.required));
  }

  removeTag(index: number): void {
    this.tags.removeAt(index);
  }

  onSubmit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    this.errors.set([]);
    this.loading.set(true);

    if (this.isEditMode() && this.contactId !== null) {
      const dto = {
        firstName: this.form.value.firstName,
        lastName: this.form.value.lastName,
        note: this.form.value.note,
        address: this.form.value.address
      };

      this.contactService.updateContact(this.contactId, dto).subscribe({
        next: () => this.router.navigate(['/contacts', this.contactId]),
        error: (err) => this.handleError(err)
      });
    } else {
      const dto = this.form.value;

      this.contactService.createContact(dto).subscribe({
        next: (created) => this.router.navigate(['/contacts', created.id]),
        error: (err) => this.handleError(err)
      });
    }
  }

  private handleError(err: any): void {
    this.loading.set(false);
    if (err.status === 400 && Array.isArray(err.error)) {
      this.errors.set(err.error);
    } else {
      this.errors.set(['Došlo je do greške. Pokušajte ponovno.']);
    }
  }
}