import { Address } from './address';
import { PhoneNumber } from './phone-number';
import { Email } from './email';
import { Tag } from './tag';

export interface Contact {
  id: number;
  firstName: string;
  lastName: string;
  note?: string;
  address: Address;
  phoneNumbers: PhoneNumber[];
  emails: Email[];
  tags: Tag[];
}