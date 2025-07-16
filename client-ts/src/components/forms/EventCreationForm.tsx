import { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { 
  Calendar, 
  MapPin, 
  Heart, 
  Palette, 
  ArrowLeft, 
  ArrowRight, 
  Check,
  Baby,
  Cake,
  GraduationCap,
  Gift,
  PartyPopper,
  Church
} from 'lucide-react';

export enum EventType {
  Wedding = 1,
  Engagement = 2,
  Baptism = 3,
  FirstCommunion = 4,
  Quinceañera = 5,
  BarMitzvah = 6,
  BatMitzvah = 7,
  BirthdayParty = 8,
  Anniversary = 9,
  Graduation = 10,
  BabyShower = 11,
  BridalShower = 12,
  Other = 99
}

interface EventFormData {
  title: string;
  eventType: EventType;
  eventDate: string;
  location: string;
  description: string;
  theme: string;
  customFields: Record<string, any>;
}

interface EventTypeOption {
  type: EventType;
  label: string;
  icon: any;
  description: string;
  color: string;
}

const eventTypeOptions: EventTypeOption[] = [
  {
    type: EventType.Wedding,
    label: 'Wedding',
    icon: Heart,
    description: 'Celebrate your special day',
    color: 'rose'
  },
  {
    type: EventType.Engagement,
    label: 'Engagement',
    icon: Heart,
    description: 'Announce your engagement',
    color: 'pink'
  },
  {
    type: EventType.Baptism,
    label: 'Baptism',
    icon: Church,
    description: 'Welcome to the faith',
    color: 'blue'
  },
  {
    type: EventType.FirstCommunion,
    label: 'First Communion',
    icon: Church,
    description: 'First holy communion celebration',
    color: 'indigo'
  },
  {
    type: EventType.Quinceañera,
    label: 'Quinceañera',
    icon: PartyPopper,
    description: 'Celebrate the 15th birthday',
    color: 'purple'
  },
  {
    type: EventType.BirthdayParty,
    label: 'Birthday Party',
    icon: Cake,
    description: 'Birthday celebration',
    color: 'yellow'
  },
  {
    type: EventType.BabyShower,
    label: 'Baby Shower',
    icon: Baby,
    description: 'Welcome the new baby',
    color: 'green'
  },
  {
    type: EventType.Graduation,
    label: 'Graduation',
    icon: GraduationCap,
    description: 'Celebrate achievements',
    color: 'emerald'
  }
];

const colorThemes = [
  { name: 'Rose Gold', value: 'rose-gold', color: 'bg-gradient-to-r from-rose-300 to-pink-300' },
  { name: 'Classic Blue', value: 'classic-blue', color: 'bg-gradient-to-r from-blue-400 to-blue-600' },
  { name: 'Sage Green', value: 'sage-green', color: 'bg-gradient-to-r from-green-300 to-emerald-400' },
  { name: 'Lavender', value: 'lavender', color: 'bg-gradient-to-r from-purple-300 to-pink-300' },
  { name: 'Champagne', value: 'champagne', color: 'bg-gradient-to-r from-yellow-200 to-amber-300' },
  { name: 'Dusty Blue', value: 'dusty-blue', color: 'bg-gradient-to-r from-slate-300 to-blue-400' }
];

function EventCreationForm() {
  const navigate = useNavigate();
  const [currentStep, setCurrentStep] = useState(1);
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [formData, setFormData] = useState<EventFormData>({
    title: '',
    eventType: EventType.Wedding,
    eventDate: '',
    location: '',
    description: '',
    theme: 'rose-gold',
    customFields: {}
  });

  const [errors, setErrors] = useState<Record<string, string>>({});

  const validateStep = (step: number): boolean => {
    const newErrors: Record<string, string> = {};

    if (step === 1) {
      if (!formData.eventType) newErrors.eventType = 'Please select an event type';
    }
    
    if (step === 2) {
      if (!formData.title.trim()) newErrors.title = 'Event title is required';
      if (!formData.eventDate) newErrors.eventDate = 'Event date is required';
      if (!formData.location.trim()) newErrors.location = 'Location is required';
    }

    setErrors(newErrors);
    return Object.keys(newErrors).length === 0;
  };

  const handleNext = () => {
    if (validateStep(currentStep)) {
      setCurrentStep(prev => prev + 1);
    }
  };

  const handlePrev = () => {
    setCurrentStep(prev => prev - 1);
    setErrors({});
  };

  const handleSubmit = async () => {
    if (!validateStep(currentStep)) return;

    setIsSubmitting(true);
    try {
      // Generate slug from title
      const slug = formData.title.toLowerCase()
        .replace(/[^a-z0-9]+/g, '-')
        .replace(/(^-|-$)/g, '');

      const eventData = {
        title: formData.title,
        slug,
        eventDate: new Date(formData.eventDate).toISOString(),
        location: formData.location,
        description: formData.description,
        eventType: formData.eventType,
        theme: formData.theme,
        customFields: JSON.stringify(formData.customFields) // Convert to JSON string
      };

      const response = await fetch('https://localhost:7193/api/events', {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
          'Authorization': `Bearer ${localStorage.getItem('token')}`
        },
        body: JSON.stringify(eventData)
      });

      if (response.ok) {
        navigate('/dashboard');
      } else {
        const error = await response.text();
        setErrors({ submit: error || 'Failed to create event' });
      }
    } catch (error) {
      setErrors({ submit: 'Network error. Please try again.' });
    } finally {
      setIsSubmitting(false);
    }
  };

  const getStepTitle = (step: number): string => {
    switch (step) {
      case 1: return 'Choose Event Type';
      case 2: return 'Event Details';
      case 3: return 'Customize & Review';
      default: return '';
    }
  };

  const selectedEventType = eventTypeOptions.find(option => option.type === formData.eventType);

  return (
    <div className="min-h-screen bg-gray-50">
      {/* Header */}
      <header className="bg-white shadow-sm border-b">
        <div className="max-w-4xl mx-auto px-4 sm:px-6 lg:px-8">
          <div className="flex justify-between items-center py-4">
            <button
              onClick={() => navigate('/dashboard')}
              className="flex items-center text-gray-600 hover:text-gray-900 transition-colors"
            >
              <ArrowLeft className="w-5 h-5 mr-2" />
              Back to Dashboard
            </button>
            <h1 className="text-2xl font-bold text-gray-900">Create New Event</h1>
            <div className="w-24"></div> {/* Spacer for centering */}
          </div>
        </div>
      </header>

      <div className="max-w-4xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
        {/* Progress Bar */}
        <div className="mb-8">
          <div className="flex items-center justify-between mb-4">
            <h2 className="text-lg font-semibold text-gray-900">{getStepTitle(currentStep)}</h2>
            <span className="text-sm text-gray-500">Step {currentStep} of 3</span>
          </div>
          <div className="w-full bg-gray-200 rounded-full h-2">
            <div 
              className="bg-gradient-to-r from-rose-500 to-pink-600 h-2 rounded-full transition-all duration-300"
              style={{ width: `${(currentStep / 3) * 100}%` }}
            ></div>
          </div>
        </div>

        <div className="bg-white rounded-2xl shadow-sm border p-8">
          {/* Step 1: Event Type Selection */}
          {currentStep === 1 && (
            <div>
              <div className="text-center mb-8">
                <h3 className="text-2xl font-bold text-gray-900 mb-2">What are you celebrating?</h3>
                <p className="text-gray-600">Choose the type of event you're planning</p>
              </div>

              <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
                {eventTypeOptions.map((option) => {
                  const Icon = option.icon;
                  const isSelected = formData.eventType === option.type;
                  
                  return (
                    <button
                      key={option.type}
                      onClick={() => setFormData({ ...formData, eventType: option.type })}
                      className={`p-6 rounded-xl border-2 transition-all text-left hover:shadow-md ${
                        isSelected 
                          ? `border-${option.color}-500 bg-${option.color}-50 ring-2 ring-${option.color}-200` 
                          : 'border-gray-200 hover:border-gray-300'
                      }`}
                    >
                      <div className={`inline-flex p-3 rounded-lg mb-4 ${
                        isSelected ? `bg-${option.color}-100` : 'bg-gray-100'
                      }`}>
                        <Icon className={`w-6 h-6 ${
                          isSelected ? `text-${option.color}-600` : 'text-gray-600'
                        }`} />
                      </div>
                      <h4 className="font-semibold text-gray-900 mb-1">{option.label}</h4>
                      <p className="text-sm text-gray-600">{option.description}</p>
                    </button>
                  );
                })}
              </div>

              {errors.eventType && (
                <p className="mt-4 text-sm text-red-600">{errors.eventType}</p>
              )}
            </div>
          )}

          {/* Step 2: Event Details */}
          {currentStep === 2 && (
            <div>
              <div className="text-center mb-8">
                <div className={`inline-flex p-3 rounded-lg mb-4 bg-${selectedEventType?.color}-100`}>
                  {selectedEventType && <selectedEventType.icon className={`w-8 h-8 text-${selectedEventType.color}-600`} />}
                </div>
                <h3 className="text-2xl font-bold text-gray-900 mb-2">Tell us about your {selectedEventType?.label.toLowerCase()}</h3>
                <p className="text-gray-600">Provide the essential details for your event</p>
              </div>

              <div className="space-y-6">
                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-2">
                    Event Title *
                  </label>
                  <input
                    type="text"
                    value={formData.title}
                    onChange={(e) => setFormData({ ...formData, title: e.target.value })}
                    placeholder={`Enter your ${selectedEventType?.label.toLowerCase()} title`}
                    className={`w-full px-4 py-3 border border-gray-300 rounded-lg focus:ring-2 focus:ring-rose-500 focus:border-transparent ${
                      errors.title ? 'border-red-300' : ''
                    }`}
                  />
                  {errors.title && <p className="mt-1 text-sm text-red-600">{errors.title}</p>}
                </div>

                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-2">
                    <Calendar className="w-4 h-4 inline mr-2" />
                    Event Date & Time *
                  </label>
                  <input
                    type="datetime-local"
                    value={formData.eventDate}
                    onChange={(e) => setFormData({ ...formData, eventDate: e.target.value })}
                    min={new Date().toISOString().slice(0, 16)}
                    className={`w-full px-4 py-3 border border-gray-300 rounded-lg focus:ring-2 focus:ring-rose-500 focus:border-transparent ${
                      errors.eventDate ? 'border-red-300' : ''
                    }`}
                  />
                  {errors.eventDate && <p className="mt-1 text-sm text-red-600">{errors.eventDate}</p>}
                </div>

                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-2">
                    <MapPin className="w-4 h-4 inline mr-2" />
                    Location *
                  </label>
                  <input
                    type="text"
                    value={formData.location}
                    onChange={(e) => setFormData({ ...formData, location: e.target.value })}
                    placeholder="Enter the event location"
                    className={`w-full px-4 py-3 border border-gray-300 rounded-lg focus:ring-2 focus:ring-rose-500 focus:border-transparent ${
                      errors.location ? 'border-red-300' : ''
                    }`}
                  />
                  {errors.location && <p className="mt-1 text-sm text-red-600">{errors.location}</p>}
                </div>

                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-2">
                    Description
                  </label>
                  <textarea
                    value={formData.description}
                    onChange={(e) => setFormData({ ...formData, description: e.target.value })}
                    placeholder="Tell guests more about your special day..."
                    rows={4}
                    className="w-full px-4 py-3 border border-gray-300 rounded-lg focus:ring-2 focus:ring-rose-500 focus:border-transparent"
                  />
                </div>
              </div>
            </div>
          )}

          {/* Step 3: Customize & Review */}
          {currentStep === 3 && (
            <div>
              <div className="text-center mb-8">
                <div className={`inline-flex p-3 rounded-lg mb-4 bg-${selectedEventType?.color}-100`}>
                  <Palette className={`w-8 h-8 text-${selectedEventType?.color}-600`} />
                </div>
                <h3 className="text-2xl font-bold text-gray-900 mb-2">Customize Your Theme</h3>
                <p className="text-gray-600">Choose colors and review your event details</p>
              </div>

              <div className="space-y-8">
                {/* Theme Selection */}
                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-4">
                    Color Theme
                  </label>
                  <div className="grid grid-cols-2 md:grid-cols-3 gap-4">
                    {colorThemes.map((theme) => (
                      <button
                        key={theme.value}
                        onClick={() => setFormData({ ...formData, theme: theme.value })}
                        className={`p-4 rounded-lg border-2 transition-all ${
                          formData.theme === theme.value 
                            ? 'border-rose-500 ring-2 ring-rose-200' 
                            : 'border-gray-200 hover:border-gray-300'
                        }`}
                      >
                        <div className={`w-full h-8 rounded-md mb-2 ${theme.color}`}></div>
                        <p className="text-sm font-medium text-gray-900">{theme.name}</p>
                      </button>
                    ))}
                  </div>
                </div>

                {/* Event Summary */}
                <div className="bg-gray-50 rounded-lg p-6">
                  <h4 className="font-semibold text-gray-900 mb-4">Event Summary</h4>
                  <div className="space-y-3">
                    <div className="flex justify-between">
                      <span className="text-gray-600">Type:</span>
                      <span className="font-medium">{selectedEventType?.label}</span>
                    </div>
                    <div className="flex justify-between">
                      <span className="text-gray-600">Title:</span>
                      <span className="font-medium">{formData.title}</span>
                    </div>
                    <div className="flex justify-between">
                      <span className="text-gray-600">Date:</span>
                      <span className="font-medium">
                        {formData.eventDate ? new Date(formData.eventDate).toLocaleDateString('en-US', {
                          weekday: 'long',
                          year: 'numeric',
                          month: 'long',
                          day: 'numeric',
                          hour: '2-digit',
                          minute: '2-digit'
                        }) : 'Not set'}
                      </span>
                    </div>
                    <div className="flex justify-between">
                      <span className="text-gray-600">Location:</span>
                      <span className="font-medium">{formData.location}</span>
                    </div>
                    <div className="flex justify-between">
                      <span className="text-gray-600">Theme:</span>
                      <span className="font-medium">{colorThemes.find(t => t.value === formData.theme)?.name}</span>
                    </div>
                  </div>
                </div>

                {errors.submit && (
                  <div className="bg-red-50 border border-red-200 rounded-lg p-4">
                    <p className="text-sm text-red-600">{errors.submit}</p>
                  </div>
                )}
              </div>
            </div>
          )}

          {/* Navigation Buttons */}
          <div className="flex justify-between mt-8 pt-6 border-t border-gray-200">
            <button
              onClick={handlePrev}
              disabled={currentStep === 1}
              className={`flex items-center px-6 py-3 rounded-lg font-medium transition-colors ${
                currentStep === 1
                  ? 'text-gray-400 cursor-not-allowed'
                  : 'text-gray-700 hover:text-gray-900 hover:bg-gray-100'
              }`}
            >
              <ArrowLeft className="w-5 h-5 mr-2" />
              Previous
            </button>

            {currentStep < 3 ? (
              <button
                onClick={handleNext}
                className="flex items-center px-6 py-3 bg-gradient-to-r from-rose-500 to-pink-600 text-white rounded-lg font-medium hover:from-rose-600 hover:to-pink-700 transition-colors"
              >
                Next
                <ArrowRight className="w-5 h-5 ml-2" />
              </button>
            ) : (
              <button
                onClick={handleSubmit}
                disabled={isSubmitting}
                className="flex items-center px-6 py-3 bg-gradient-to-r from-rose-500 to-pink-600 text-white rounded-lg font-medium hover:from-rose-600 hover:to-pink-700 transition-colors disabled:opacity-50 disabled:cursor-not-allowed"
              >
                {isSubmitting ? (
                  <>
                    <div className="w-5 h-5 mr-2 border-2 border-white border-t-transparent rounded-full animate-spin"></div>
                    Creating...
                  </>
                ) : (
                  <>
                    <Check className="w-5 h-5 mr-2" />
                    Create Event
                  </>
                )}
              </button>
            )}
          </div>
        </div>
      </div>
    </div>
  );
}

export default EventCreationForm;