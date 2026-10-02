import { AddressWrite } from './address-write';

export interface PutContact {
  firstName: string;
  lastName: string;
  note?: string;
  address: AddressWrite;
}