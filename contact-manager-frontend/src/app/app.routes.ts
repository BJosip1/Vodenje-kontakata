import { Routes } from '@angular/router';
import { ContactList } from './components/contact-list/contact-list';
import { ContactDetail } from './components/contact-detail/contact-detail';
import { ContactForm } from './components/contact-form/contact-form';

export const routes: Routes = [
  { path: 'contacts', component: ContactList },
  { path: 'contacts/new', component: ContactForm },
  { path: 'contacts/:id/edit', component: ContactForm },
  { path: 'contacts/:id', component: ContactDetail },
  { path: '', redirectTo: 'contacts', pathMatch: 'full' }
];