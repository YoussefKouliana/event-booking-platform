import api from './axios';
import type { Package, PriceBreakdown, PackageType } from '../types';

export const packagesApi = {
  getPackages: () => api.get<Package[]>('/events/packages'),
  
  calculatePrice: (packageType: PackageType, addOns: string[]) => 
    api.post<PriceBreakdown>('/events/calculate-price', { 
      packageType, 
      addOns 
    })
};