import { PhoneNumberWrite } from './phone-number-write';
import { EmailWrite } from './email-write';

export interface AddContactDetails {
  phoneNumbers: PhoneNumberWrite[];
  emails: EmailWrite[];
  tags: string[];
}