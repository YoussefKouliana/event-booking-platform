import { useState, useEffect } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { 
  ArrowLeft, 
  Calendar, 
  MapPin, 
  Users, 
  Eye, 
  Share2, 
  Settings,
  BarChart3,
  Heart,
  Clock,
  Edit,
  ExternalLink
} from 'lucide-react';

// Import your existing tab components
import RsvpManagementTab from '../components/eventTabs/RsvpManagementTab';
import SharingCountdownTab from '../components/eventTabs/SharingCountdownTab';

interface Event {
  id: number;
  title: string;
  slug: string;
  eventDate: string;
  location: string;
  description: string;
  eventType: number;
  eventTypeName: string;
  theme: string;
  customFields: string | null;
  createdAt: string;
  updatedAt: string;
  totalGuests: number;
  confirmedRsvps: number;
  pendingRsvps: number;
  declinedRsvps: number;
}

export default function EventDetailsPage() {
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();
  const [event, setEvent] = useState<Event | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [activeTab, setActiveTab] = useState<'overview' | 'guests' | 'sharing' | 'settings'>('overview');

  useEffect(() => {
    if (id) {
      fetchEvent(id);
    }
  }, [id]);

  const fetchEvent = async (eventId: string) => {
    try {
      const response = await fetch(`/api/events/${eventId}`, {
        headers: {
          'Authorization': `Bearer ${localStorage.getItem('token')}`
        }
      });

      if (response.ok) {
        const data = await response.json();
        setEvent(data);
      } else {
        setError('Event not found');
      }
    } catch (err) {
      setError('Failed to load event');
    } finally {
      setLoading(false);
    }
  };

  const formatEventDate = (dateString: string): string => {
    const date = new Date(dateString);
    return date.toLocaleDateString('en-US', {
      weekday: 'long',
      year: 'numeric',
      month: 'long',
      day: 'numeric',
      hour: '2-digit',
      minute: '2-digit'
    });
  };

  const formatShortDate = (dateString: string): string => {
    const date = new Date(dateString);
    return date.toLocaleDateString('en-US', {
      month: 'short',
      day: 'numeric',
      year: 'numeric'
    });
  };

  const getDaysUntilEvent = (eventDate: string): number => {
    const now = new Date();
    const event = new Date(eventDate);
    const diffTime = event.getTime() - now.getTime();
    return Math.ceil(diffTime / (1000 * 60 * 60 * 24));
  };

  const getEventTypeColor = (eventType: number): string => {
    const colors: Record<number, string> = {
      1: 'rose', 2: 'pink', 3: 'blue', 4: 'indigo', 5: 'purple',
      6: 'emerald', 7: 'emerald', 8: 'yellow', 9: 'rose', 
      10: 'green', 11: 'green', 12: 'pink'
    };
    return colors[eventType] || 'gray';
  };

  const handlePreview = () => {
    navigate(`/events/${id}/preview`);
  };

  if (loading) {
    return (
      <div className="min-h-screen bg-gray-50 flex items-center justify-center">
        <div className="text-center">
          <div className="w-8 h-8 border-4 border-rose-500 border-t-transparent rounded-full animate-spin mx-auto mb-4"></div>
          <p className="text-gray-600">Loading event...</p>
        </div>
      </div>
    );
  }

  if (error || !event) {
    return (
      <div className="min-h-screen bg-gray-50 flex items-center justify-center">
        <div className="text-center">
          <Heart className="w-16 h-16 text-gray-400 mx-auto mb-4" />
          <h2 className="text-xl font-semibold text-gray-900 mb-2">Event Not Found</h2>
          <p className="text-gray-600 mb-4">{error || 'This event could not be loaded'}</p>
          <button
            onClick={() => navigate('/dashboard')}
            className="text-rose-600 hover:text-rose-700 font-medium"
          >
            Back to Dashboard
          </button>
        </div>
      </div>
    );
  }

  const colorClass = getEventTypeColor(event.eventType);
  const daysUntil = getDaysUntilEvent(event.eventDate);
  const isUpcoming = daysUntil > 0;
  const responseRate = event.totalGuests > 0 ? 
    Math.round(((event.confirmedRsvps + event.declinedRsvps) / event.totalGuests) * 100) : 0;

  return (
    <div className="min-h-screen bg-gray-50">
      {/* Header */}
      <header className="bg-white shadow-sm border-b">
        <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
          <div className="flex items-center justify-between py-4">
            <div className="flex items-center">
              <button
                onClick={() => navigate('/dashboard')}
                className="flex items-center text-gray-600 hover:text-gray-900 transition-colors mr-4"
              >
                <ArrowLeft className="w-5 h-5 mr-2" />
                Back to Dashboard
              </button>
              <div className={`inline-flex px-3 py-1 rounded-full text-xs font-medium bg-${colorClass}-100 text-${colorClass}-700 mr-3`}>
                {event.eventTypeName}
              </div>
              <h1 className="text-2xl font-bold text-gray-900">{event.title}</h1>
            </div>
            
            <div className="flex items-center space-x-3">
              <button
                onClick={handlePreview}
                className="flex items-center px-4 py-2 text-gray-700 bg-gray-100 rounded-lg hover:bg-gray-200 transition-colors"
              >
                <Eye className="w-4 h-4 mr-2" />
                Preview
              </button>
              <button
                onClick={() => setActiveTab('sharing')}
                className="flex items-center px-4 py-2 bg-rose-600 text-white rounded-lg hover:bg-rose-700 transition-colors"
              >
                <Share2 className="w-4 h-4 mr-2" />
                Share
              </button>
            </div>
          </div>
        </div>
      </header>

      <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
        {/* Event Summary Cards */}
        <div className="grid grid-cols-1 md:grid-cols-4 gap-6 mb-8">
          <div className="bg-white rounded-xl p-6 shadow-sm border">
            <div className="flex items-center">
              <div className="bg-blue-100 p-3 rounded-lg mr-4">
                <Calendar className="w-6 h-6 text-blue-600" />
              </div>
              <div>
                <h3 className="text-lg font-semibold text-gray-900">
                  {isUpcoming ? `${daysUntil} days` : 'Past Event'}
                </h3>
                <p className="text-gray-600 text-sm">{formatShortDate(event.eventDate)}</p>
              </div>
            </div>
          </div>

          <div className="bg-white rounded-xl p-6 shadow-sm border">
            <div className="flex items-center">
              <div className="bg-green-100 p-3 rounded-lg mr-4">
                <Users className="w-6 h-6 text-green-600" />
              </div>
              <div>
                <h3 className="text-lg font-semibold text-gray-900">{event.totalGuests}</h3>
                <p className="text-gray-600 text-sm">Total Guests</p>
              </div>
            </div>
          </div>

          <div className="bg-white rounded-xl p-6 shadow-sm border">
            <div className="flex items-center">
              <div className="bg-purple-100 p-3 rounded-lg mr-4">
                <Heart className="w-6 h-6 text-purple-600" />
              </div>
              <div>
                <h3 className="text-lg font-semibold text-gray-900">{event.confirmedRsvps}</h3>
                <p className="text-gray-600 text-sm">Confirmed RSVPs</p>
              </div>
            </div>
          </div>

          <div className="bg-white rounded-xl p-6 shadow-sm border">
            <div className="flex items-center">
              <div className="bg-amber-100 p-3 rounded-lg mr-4">
                <BarChart3 className="w-6 h-6 text-amber-600" />
              </div>
              <div>
                <h3 className="text-lg font-semibold text-gray-900">{responseRate}%</h3>
                <p className="text-gray-600 text-sm">Response Rate</p>
              </div>
            </div>
          </div>
        </div>

        {/* Tab Navigation */}
        <div className="bg-white rounded-xl shadow-sm border mb-8">
          <div className="border-b border-gray-200">
            <nav className="flex space-x-8 px-6">
              {[
                { key: 'overview', label: 'Overview', icon: BarChart3 },
                { key: 'guests', label: 'Guests & RSVPs', icon: Users },
                { key: 'sharing', label: 'Sharing & Countdown', icon: Share2 },
                { key: 'settings', label: 'Settings', icon: Settings }
              ].map((tab) => {
                const Icon = tab.icon;
                return (
                  <button
                    key={tab.key}
                    onClick={() => setActiveTab(tab.key as any)}
                    className={`flex items-center py-4 px-1 border-b-2 font-medium text-sm transition-colors ${
                      activeTab === tab.key
                        ? 'border-rose-500 text-rose-600'
                        : 'border-transparent text-gray-500 hover:text-gray-700 hover:border-gray-300'
                    }`}
                  >
                    <Icon className="w-4 h-4 mr-2" />
                    {tab.label}
                  </button>
                );
              })}
            </nav>
          </div>

          {/* Tab Content */}
          <div className="p-6">
            {activeTab === 'overview' && (
              <div className="space-y-6">
                <div className="grid grid-cols-1 lg:grid-cols-2 gap-6">
                  {/* Event Details */}
                  <div>
                    <h3 className="text-lg font-semibold text-gray-900 mb-4">Event Details</h3>
                    <div className="space-y-4">
                      <div className="flex items-center text-gray-700">
                        <Calendar className="w-5 h-5 mr-3 text-gray-500" />
                        <span>{formatEventDate(event.eventDate)}</span>
                      </div>
                      <div className="flex items-center text-gray-700">
                        <MapPin className="w-5 h-5 mr-3 text-gray-500" />
                        <span>{event.location}</span>
                      </div>
                      {event.description && (
                        <div className="pt-2">
                          <p className="text-gray-600">{event.description}</p>
                        </div>
                      )}
                    </div>
                  </div>

                  {/* Quick Stats */}
                  <div>
                    <h3 className="text-lg font-semibold text-gray-900 mb-4">RSVP Status</h3>
                    <div className="space-y-3">
                      <div className="flex justify-between items-center">
                        <span className="text-gray-600">Confirmed</span>
                        <span className="font-semibold text-green-600">{event.confirmedRsvps}</span>
                      </div>
                      <div className="flex justify-between items-center">
                        <span className="text-gray-600">Pending</span>
                        <span className="font-semibold text-amber-600">{event.pendingRsvps}</span>
                      </div>
                      <div className="flex justify-between items-center">
                        <span className="text-gray-600">Declined</span>
                        <span className="font-semibold text-red-600">{event.declinedRsvps}</span>
                      </div>
                      <div className="border-t pt-3 flex justify-between items-center">
                        <span className="text-gray-900 font-semibold">Total</span>
                        <span className="font-semibold">{event.totalGuests}</span>
                      </div>
                    </div>
                  </div>
                </div>

                {/* Quick Actions */}
                <div className="border-t pt-6">
                  <h3 className="text-lg font-semibold text-gray-900 mb-4">Quick Actions</h3>
                  <div className="grid grid-cols-1 md:grid-cols-3 gap-4">
                    <button
                      onClick={() => setActiveTab('guests')}
                      className="flex items-center justify-center p-4 border border-gray-200 rounded-lg hover:bg-gray-50 transition-colors"
                    >
                      <Users className="w-5 h-5 mr-2 text-gray-600" />
                      Manage Guests
                    </button>
                    <button
                      onClick={handlePreview}
                      className="flex items-center justify-center p-4 border border-gray-200 rounded-lg hover:bg-gray-50 transition-colors"
                    >
                      <ExternalLink className="w-5 h-5 mr-2 text-gray-600" />
                      Preview Invitation
                    </button>
                    <button
                      onClick={() => setActiveTab('sharing')}
                      className="flex items-center justify-center p-4 border border-gray-200 rounded-lg hover:bg-gray-50 transition-colors"
                    >
                      <Share2 className="w-5 h-5 mr-2 text-gray-600" />
                      Share Event
                    </button>
                  </div>
                </div>
              </div>
            )}

            {activeTab === 'guests' && (
              <RsvpManagementTab eventId={event.id} />
            )}

            {activeTab === 'sharing' && (
              <SharingCountdownTab event={event} />
            )}

            {activeTab === 'settings' && (
              <div className="text-center py-12">
                <Settings className="w-16 h-16 text-gray-400 mx-auto mb-4" />
                <h3 className="text-lg font-semibold text-gray-900 mb-2">Event Settings</h3>
                <p className="text-gray-600">Event settings panel coming soon...</p>
              </div>
            )}
          </div>
        </div>
      </div>
    </div>
  );
}