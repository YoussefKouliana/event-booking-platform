import { useState, useEffect } from 'react';
import { useParams } from 'react-router-dom';
import { Calendar, MapPin, Clock, Heart, Gift, Users } from 'lucide-react';

interface PreviewData {
  event: {
    id: number;
    title: string;
    eventType: number;
    eventTypeName: string;
    theme: string;
  };
  customData: {
    celebrantNames: {
      primary: string;
      secondary?: string;
    };
    ceremony: {
      date: string;
      time: string;
      location: string;
      address: string;
    };
    reception: {
      date: string;
      time: string;
      location: string;
      address: string;
    };
    photos: {
      heroImage?: string;
      additionalPhotos: string[];
    };
    messages: {
      welcomeMessage: string;
      ceremonyMessage: string;
      receptionMessage: string;
    };
    dressCode?: string;
    giftRegistry?: string;
  };
}

function PreviewPage() {
  const { id } = useParams<{ id: string }>();
  const [previewData, setPreviewData] = useState<PreviewData | null>(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    // Try to get preview data from localStorage first
    const storedData = localStorage.getItem('previewData');
    if (storedData) {
      try {
        const parsed = JSON.parse(storedData);
        setPreviewData(parsed);
        setLoading(false);
        return;
      } catch (e) {
        console.warn('Failed to parse preview data');
      }
    }

    // If no stored data, fetch from API
    fetchEventData();
  }, [id]);

  const fetchEventData = async () => {
    try {
      const response = await fetch(`/api/events/${id}`);
      if (response.ok) {
        const eventData = await response.json();
        const customData = eventData.customFields ? JSON.parse(eventData.customFields) : {};
        
        setPreviewData({
          event: eventData,
          customData: customData
        });
      }
    } catch (err) {
      console.error('Failed to fetch event data:', err);
    } finally {
      setLoading(false);
    }
  };

  const formatDate = (dateString: string) => {
    if (!dateString) return '';
    const date = new Date(dateString);
    return date.toLocaleDateString('en-US', {
      weekday: 'long',
      year: 'numeric',
      month: 'long',
      day: 'numeric'
    });
  };

  const formatTime = (timeString: string) => {
    if (!timeString) return '';
    const [hours, minutes] = timeString.split(':');
    const date = new Date();
    date.setHours(parseInt(hours), parseInt(minutes));
    return date.toLocaleTimeString('en-US', {
      hour: 'numeric',
      minute: '2-digit',
      hour12: true
    });
  };

  const getThemeColors = (theme: string) => {
    const themes = {
      'rose-gold': {
        primary: 'from-rose-400 to-pink-500',
        secondary: 'bg-rose-50',
        text: 'text-rose-800',
        accent: 'text-rose-600'
      },
      'classic-blue': {
        primary: 'from-blue-400 to-blue-600',
        secondary: 'bg-blue-50',
        text: 'text-blue-800',
        accent: 'text-blue-600'
      },
      'sage-green': {
        primary: 'from-green-400 to-emerald-500',
        secondary: 'bg-green-50',
        text: 'text-green-800',
        accent: 'text-green-600'
      },
      'lavender': {
        primary: 'from-purple-400 to-pink-400',
        secondary: 'bg-purple-50',
        text: 'text-purple-800',
        accent: 'text-purple-600'
      },
      'champagne': {
        primary: 'from-yellow-300 to-amber-400',
        secondary: 'bg-yellow-50',
        text: 'text-yellow-800',
        accent: 'text-yellow-700'
      },
      'dusty-blue': {
        primary: 'from-slate-400 to-blue-400',
        secondary: 'bg-slate-50',
        text: 'text-slate-800',
        accent: 'text-slate-600'
      }
    };
    return themes[theme as keyof typeof themes] || themes['rose-gold'];
  };

  if (loading) {
    return (
      <div className="min-h-screen bg-gray-50 flex items-center justify-center">
        <div className="text-center">
          <div className="w-8 h-8 border-4 border-rose-500 border-t-transparent rounded-full animate-spin mx-auto mb-4"></div>
          <p className="text-gray-600">Loading preview...</p>
        </div>
      </div>
    );
  }

  if (!previewData) {
    return (
      <div className="min-h-screen bg-gray-50 flex items-center justify-center">
        <div className="text-center">
          <Heart className="w-16 h-16 text-gray-400 mx-auto mb-4" />
          <h2 className="text-xl font-semibold text-gray-900 mb-2">Preview Not Available</h2>
          <p className="text-gray-600">Unable to load invitation preview</p>
        </div>
      </div>
    );
  }

  const { event, customData } = previewData;
  const colors = getThemeColors(event.theme);
  const ceremonyDate = formatDate(customData.ceremony?.date);
  const ceremonyTime = formatTime(customData.ceremony?.time);
  const receptionDate = formatDate(customData.reception?.date);
  const receptionTime = formatTime(customData.reception?.time);

  return (
    <div className="min-h-screen bg-gray-100 py-8">
      <div className="max-w-2xl mx-auto">
        {/* Invitation Card */}
        <div className="bg-white rounded-2xl shadow-2xl overflow-hidden">
          {/* Hero Section */}
          <div className={`bg-gradient-to-br ${colors.primary} p-8 text-white text-center relative`}>
            {customData.photos?.heroImage && (
              <div className="absolute inset-0">
                <img 
                  src={customData.photos.heroImage}
                  alt="Hero"
                  className="w-full h-full object-cover opacity-30"
                />
                <div className="absolute inset-0 bg-gradient-to-br from-black/40 to-black/20"></div>
              </div>
            )}
            
            <div className="relative z-10">
              <Heart className="w-12 h-12 mx-auto mb-4 opacity-80" />
              <h1 className="text-3xl md:text-4xl font-bold mb-2">
                {customData.celebrantNames?.primary}
                {customData.celebrantNames?.secondary && (
                  <> & {customData.celebrantNames.secondary}</>
                )}
              </h1>
              <p className="text-xl opacity-90">
                {event.eventTypeName}
              </p>
              {ceremonyDate && (
                <p className="text-lg mt-2 opacity-80">
                  {ceremonyDate}
                </p>
              )}
            </div>
          </div>

          {/* Welcome Message */}
          {customData.messages?.welcomeMessage && (
            <div className="p-8 text-center">
              <p className="text-gray-700 text-lg leading-relaxed italic">
                "{customData.messages.welcomeMessage}"
              </p>
            </div>
          )}

          {/* Event Details */}
          <div className="p-8 space-y-8">
            {/* Ceremony */}
            {customData.ceremony?.location && (
              <div className={`${colors.secondary} rounded-xl p-6`}>
                <h3 className={`text-xl font-semibold ${colors.text} mb-4 flex items-center`}>
                  <Calendar className="w-5 h-5 mr-2" />
                  Ceremony
                </h3>
                
                {customData.messages?.ceremonyMessage && (
                  <p className="text-gray-600 mb-4 italic">
                    {customData.messages.ceremonyMessage}
                  </p>
                )}
                
                <div className="space-y-3">
                  {ceremonyDate && (
                    <div className="flex items-center text-gray-700">
                      <Calendar className="w-4 h-4 mr-3 text-gray-500" />
                      <span>{ceremonyDate}</span>
                    </div>
                  )}
                  
                  {ceremonyTime && (
                    <div className="flex items-center text-gray-700">
                      <Clock className="w-4 h-4 mr-3 text-gray-500" />
                      <span>{ceremonyTime}</span>
                    </div>
                  )}
                  
                  <div className="flex items-start text-gray-700">
                    <MapPin className="w-4 h-4 mr-3 mt-1 text-gray-500 flex-shrink-0" />
                    <div>
                      <p className="font-medium">{customData.ceremony.location}</p>
                      {customData.ceremony.address && (
                        <p className="text-sm text-gray-600 mt-1">
                          {customData.ceremony.address}
                        </p>
                      )}
                    </div>
                  </div>
                </div>
              </div>
            )}

            {/* Reception */}
            {customData.reception?.location && (
              <div className={`${colors.secondary} rounded-xl p-6`}>
                <h3 className={`text-xl font-semibold ${colors.text} mb-4 flex items-center`}>
                  <Users className="w-5 h-5 mr-2" />
                  Reception
                </h3>
                
                {customData.messages?.receptionMessage && (
                  <p className="text-gray-600 mb-4 italic">
                    {customData.messages.receptionMessage}
                  </p>
                )}
                
                <div className="space-y-3">
                  {receptionDate && (
                    <div className="flex items-center text-gray-700">
                      <Calendar className="w-4 h-4 mr-3 text-gray-500" />
                      <span>{receptionDate}</span>
                    </div>
                  )}
                  
                  {receptionTime && (
                    <div className="flex items-center text-gray-700">
                      <Clock className="w-4 h-4 mr-3 text-gray-500" />
                      <span>{receptionTime}</span>
                    </div>
                  )}
                  
                  <div className="flex items-start text-gray-700">
                    <MapPin className="w-4 h-4 mr-3 mt-1 text-gray-500 flex-shrink-0" />
                    <div>
                      <p className="font-medium">{customData.reception.location}</p>
                      {customData.reception.address && (
                        <p className="text-sm text-gray-600 mt-1">
                          {customData.reception.address}
                        </p>
                      )}
                    </div>
                  </div>
                </div>
              </div>
            )}

            {/* Additional Info */}
            {(customData.dressCode || customData.giftRegistry) && (
              <div className="border-t pt-6">
                <h4 className="font-semibold text-gray-900 mb-4">Additional Information</h4>
                <div className="space-y-3 text-gray-700">
                  {customData.dressCode && (
                    <p>
                      <span className="font-medium">Dress Code:</span> {customData.dressCode}
                    </p>
                  )}
                  
                  {customData.giftRegistry && (
                    <div className="flex items-center">
                      <Gift className="w-4 h-4 mr-2 text-gray-500" />
                      <a 
                        href={customData.giftRegistry}
                        target="_blank"
                        rel="noopener noreferrer"
                        className={`${colors.accent} hover:underline`}
                      >
                        View Gift Registry
                      </a>
                    </div>
                  )}
                </div>
              </div>
            )}

            {/* Additional Photos */}
            {customData.photos?.additionalPhotos?.length > 0 && (
              <div className="border-t pt-6">
                <h4 className="font-semibold text-gray-900 mb-4">Photos</h4>
                <div className="grid grid-cols-2 md:grid-cols-3 gap-4">
                  {customData.photos.additionalPhotos.map((photo, index) => (
                    <img
                      key={index}
                      src={photo}
                      alt={`Photo ${index + 1}`}
                      className="w-full h-24 object-cover rounded-lg"
                    />
                  ))}
                </div>
              </div>
            )}
          </div>

          {/* Footer */}
          <div className={`bg-gradient-to-r ${colors.primary} p-6 text-center text-white`}>
            <Heart className="w-8 h-8 mx-auto mb-2 opacity-80" />
            <p className="text-sm opacity-90">
              We can't wait to celebrate with you!
            </p>
          </div>
        </div>

        {/* Action Button */}
        <div className="text-center mt-8">
          <button
            onClick={() => window.close()}
            className="px-6 py-3 bg-gray-600 text-white rounded-lg hover:bg-gray-700 transition-colors"
          >
            Close Preview
          </button>
        </div>
      </div>
    </div>
  );
}

export default PreviewPage;