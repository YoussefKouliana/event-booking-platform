import { useState, useEffect } from 'react';
import { Check, Star, Info, DollarSign } from 'lucide-react';
import { packagesApi } from '../../api/package';
import type { Package, AddOn, PriceBreakdown, PackageType } from '../../types';

interface PackageSelectorProps {
  selectedPackage: PackageType;
  selectedAddOns: string[];
  onPackageChange: (packageType: PackageType) => void;
  onAddOnsChange: (addOns: string[]) => void;
  onPriceChange: (totalPrice: number) => void;
}

function PackageSelector({
  selectedPackage,
  selectedAddOns,
  onPackageChange,
  onAddOnsChange,
  onPriceChange
}: PackageSelectorProps) {
  const [packages, setPackages] = useState<Package[]>([]);
  const [priceBreakdown, setPriceBreakdown] = useState<PriceBreakdown | null>(null);
  const [loading, setLoading] = useState(true);
  const [calculatingPrice, setCalculatingPrice] = useState(false);

  useEffect(() => {
    fetchPackages();
  }, []);

  useEffect(() => {
    calculatePrice();
  }, [selectedPackage, selectedAddOns]);

  const fetchPackages = async () => {
    try {
      const response = await packagesApi.getPackages();
      setPackages(response.data);
    } catch (error) {
      console.error('Failed to fetch packages:', error);
    } finally {
      setLoading(false);
    }
  };

  const calculatePrice = async () => {
    setCalculatingPrice(true);
    try {
      const response = await packagesApi.calculatePrice(selectedPackage, selectedAddOns);
      setPriceBreakdown(response.data);
      onPriceChange(response.data.totalPrice);
    } catch (error) {
      console.error('Failed to calculate price:', error);
    } finally {
      setCalculatingPrice(false);
    }
  };

  const handlePackageSelect = (packageType: PackageType) => {
    onPackageChange(packageType);
    // Reset add-ons when package changes
    onAddOnsChange([]);
  };

  const handleAddOnToggle = (addOnKey: string) => {
    const newAddOns = selectedAddOns.includes(addOnKey)
      ? selectedAddOns.filter(key => key !== addOnKey)
      : [...selectedAddOns, addOnKey];
    onAddOnsChange(newAddOns);
  };

  const selectedPackageData = packages.find(p => p.id === selectedPackage);

  if (loading) {
    return (
      <div className="flex justify-center py-12">
        <div className="w-8 h-8 border-4 border-rose-500 border-t-transparent rounded-full animate-spin"></div>
      </div>
    );
  }

  return (
    <div className="space-y-8">
      {/* Package Selection */}
      <div>
        <h3 className="text-xl font-semibold text-gray-900 mb-6">Choose Your Package</h3>
        <div className="grid md:grid-cols-3 gap-6">
          {packages.map((pkg) => (
            <div
              key={pkg.id}
              className={`relative border-2 rounded-xl p-6 cursor-pointer transition-all ${
                selectedPackage === pkg.id
                  ? 'border-rose-500 ring-2 ring-rose-200 bg-rose-50'
                  : 'border-gray-200 hover:border-gray-300 hover:shadow-md'
              }`}
              onClick={() => handlePackageSelect(pkg.id)}
            >
              {pkg.popular && (
                <div className="absolute -top-3 left-1/2 transform -translate-x-1/2">
                  <span className="bg-rose-500 text-white px-3 py-1 rounded-full text-sm font-medium flex items-center">
                    <Star className="w-3 h-3 mr-1" />
                    Most Popular
                  </span>
                </div>
              )}

              <div className="text-center mb-4">
                <h4 className="text-xl font-bold text-gray-900 mb-2">{pkg.name}</h4>
                <div className="mb-4">
                  <span className="text-3xl font-bold text-gray-900">${pkg.price}</span>
                  <span className="text-gray-600 ml-1">one-time</span>
                </div>
              </div>

              <ul className="space-y-3 mb-6">
                {pkg.features.map((feature, index) => (
                  <li key={index} className="flex items-start">
                    <Check className="w-5 h-5 text-green-500 mr-3 mt-0.5 flex-shrink-0" />
                    <span className="text-gray-700 text-sm">{feature}</span>
                  </li>
                ))}
              </ul>

              {pkg.maxGuests && (
                <div className="text-sm text-gray-600 mb-4 p-3 bg-gray-50 rounded-lg">
                  <Info className="w-4 h-4 inline mr-2" />
                  Up to {pkg.maxGuests} guests
                </div>
              )}

              <div className={`w-full py-3 rounded-lg font-medium text-center transition-colors ${
                selectedPackage === pkg.id
                  ? 'bg-rose-500 text-white'
                  : 'bg-gray-100 text-gray-900 hover:bg-gray-200'
              }`}>
                {selectedPackage === pkg.id ? 'Selected' : 'Select Package'}
              </div>
            </div>
          ))}
        </div>
      </div>

      {/* Add-ons Selection */}
      {selectedPackageData && selectedPackageData.availableAddOns.length > 0 && (
        <div>
          <h3 className="text-xl font-semibold text-gray-900 mb-6">Enhance Your Experience</h3>
          <div className="grid md:grid-cols-2 gap-4">
            {selectedPackageData.availableAddOns.map((addOn) => {
              const isIncluded = selectedPackageData.includedAddOns.includes(addOn.key);
              const isSelected = selectedAddOns.includes(addOn.key);
              
              return (
                <div
                  key={addOn.key}
                  className={`border rounded-lg p-4 cursor-pointer transition-all ${
                    isIncluded
                      ? 'border-green-200 bg-green-50'
                      : isSelected
                      ? 'border-rose-500 bg-rose-50'
                      : 'border-gray-200 hover:border-gray-300'
                  }`}
                  onClick={() => !isIncluded && handleAddOnToggle(addOn.key)}
                >
                  <div className="flex items-start justify-between">
                    <div className="flex-1">
                      <div className="flex items-center mb-2">
                        <h4 className="font-medium text-gray-900">{addOn.name}</h4>
                        {isIncluded && (
                          <span className="ml-2 bg-green-100 text-green-700 px-2 py-1 rounded-full text-xs font-medium">
                            Included
                          </span>
                        )}
                      </div>
                      <p className="text-sm text-gray-600 mb-3">{addOn.description}</p>
                      <div className="flex items-center justify-between">
                        <span className="font-semibold text-gray-900">
                          {isIncluded ? 'Free' : `$${addOn.price}`}
                        </span>
                        <div className={`w-5 h-5 rounded border-2 flex items-center justify-center ${
                          isIncluded || isSelected
                            ? 'border-rose-500 bg-rose-500'
                            : 'border-gray-300'
                        }`}>
                          {(isIncluded || isSelected) && (
                            <Check className="w-3 h-3 text-white" />
                          )}
                        </div>
                      </div>
                    </div>
                  </div>
                </div>
              );
            })}
          </div>
        </div>
      )}

      {/* Price Summary */}
      {priceBreakdown && (
        <div className="bg-gray-50 rounded-xl p-6">
          <h3 className="text-lg font-semibold text-gray-900 mb-4 flex items-center">
            <DollarSign className="w-5 h-5 mr-2" />
            Price Summary
          </h3>
          
          <div className="space-y-3">
            <div className="flex justify-between items-center">
              <span className="text-gray-700">
                {selectedPackageData?.name} Package
              </span>
              <span className="font-medium">${priceBreakdown.packagePrice}</span>
            </div>

            {priceBreakdown.includedFeatures.length > 0 && (
              <div className="text-sm text-green-600 pl-4">
                Includes: {priceBreakdown.includedFeatures.join(', ')}
              </div>
            )}

            {priceBreakdown.addOnPrices
              .filter(addOn => !addOn.isIncluded && selectedAddOns.includes(addOn.key))
              .map((addOn) => (
                <div key={addOn.key} className="flex justify-between items-center">
                  <span className="text-gray-700">{addOn.name}</span>
                  <span className="font-medium">${addOn.price}</span>
                </div>
              ))}

            <div className="border-t pt-3 mt-3">
              <div className="flex justify-between items-center">
                <span className="text-lg font-semibold text-gray-900">Total</span>
                <span className={`text-lg font-bold ${calculatingPrice ? 'text-gray-400' : 'text-gray-900'}`}>
                  {calculatingPrice ? 'Calculating...' : `$${priceBreakdown.totalPrice}`}
                </span>
              </div>
            </div>
          </div>
        </div>
      )}
    </div>
  );
}

export default PackageSelector;