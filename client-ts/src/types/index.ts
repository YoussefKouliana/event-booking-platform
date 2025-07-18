// User types
export interface User {
  id: string;
  email: string;
  name?: string;
  firstName?: string;
  lastName?: string;
  exp: number;
}

// Event types
export interface Event {
  id: number;
  title: string;
  slug: string;
  eventDate: string;
  location?: string;
  description?: string;
  theme?: string;
  musicUrl?: string;
  isPublic: boolean;
  guestCount: number;
  rsvpCount: number;
  createdAt: string;
}

export interface EventDetail extends Event {
  guests: Guest[];
  tables: Table[];
}

export interface CreateEventDto {
  title: string;
  eventDate: string;
  location?: string;
  description?: string;
  theme?: string;
  isPublic: boolean;
}

// Guest types
export interface Guest {
  id: number;
  name: string;
  email?: string;
  customLink?: string;
  tableNumber?: string;
  rsvpStatus: string;
}

export interface CreateGuestDto {
  name: string;
  email?: string;
  tableNumber?: string;
}

// RSVP types
export interface Rsvp {
  id: number;
  status: string;
  partySize: number;
  note?: string;
  submittedAt: string;
}

export interface SubmitRsvpDto {
  customLink: string;
  status: string;
  partySize: number;
  note?: string;
}

// Table types
export interface Table {
  id: number;
  name: string;
  capacity: number;
  assignedGuests: number;
}

// API Response types
export interface ApiResponse<T> {
  success: boolean;
  data?: T;
  error?: string;
}

// Form types
export interface LoginFormData {
  email: string;
  password: string;
}

export interface RegisterFormData {
  firstName: string;
  lastName: string;
  email: string;
  password: string;
  confirmPassword: string;
}

// Validation types
export interface ValidationErrors {
  [key: string]: string;
}

// Add these new types to your existing types file

// Package types
export interface Package {
  id: PackageType;
  name: string;
  price: number;
  features: string[];
  maxGuests?: number;
  popular?: boolean;
  availableAddOns: AddOn[];
  includedAddOns: string[];
}

export interface AddOn {
  key: string;
  name: string;
  price: number;
  description: string;
}

export interface PriceBreakdown {
  packagePrice: number;
  addOnPrices: AddOnPrice[];
  totalPrice: number;
  includedFeatures: string[];
}

export interface AddOnPrice {
  key: string;
  name: string;
  price: number;
  isIncluded: boolean;
  description: string;
}

export enum PackageType {
  Essential = 1,
  Professional = 2,
  Premium = 3
}

// Update your CreateEventDto to include package info
export interface CreateEventDto {
  title: string;
  eventDate: string;
  location?: string;
  description?: string;
  theme?: string;
  isPublic: boolean;
  packageType: PackageType;
  selectedAddOns?: string[];
}