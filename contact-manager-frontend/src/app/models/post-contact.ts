import { AddressWrite } from './address-write';
import { PhoneNumberWrite } from './phone-number-write';
import { EmailWrite } from './email-write';

export interface PostContact {
  firstName: string;
  lastName: string;
  note?: string;
  address: AddressWrite;
  phoneNumbers: PhoneNumberWrite[];
  emails: EmailWrite[];
  tags: string[];
}