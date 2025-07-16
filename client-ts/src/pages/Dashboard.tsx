import { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { useAuth } from '../contexts/AuthContext';
import { Heart, Calendar, Users, LogOut, Plus, MapPin, Clock } from 'lucide-react';

interface EventListItem {
  id: number;
  title: string;
  slug: string;
  eventDate: string;
  location: string;
  eventType: number;
  eventTypeName: string;
  theme: string;
  createdAt: string;
  totalGuests: number;
  confirmedRsvps: number;
  isUpcoming: boolean;
  daysUntilEvent: number;
}

function Dashboard() {
  const { user, logout } = useAuth();
  const navigate = useNavigate();
  const [events, setEvents] = useState<EventListItem[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    fetchEvents();
  }, []);

  const fetchEvents = async () => {
    try {
      const response = await fetch('/api/events', {
        headers: {
          'Authorization': `Bearer ${localStorage.getItem('token')}`
        }
      });

      if (response.ok) {
        const data = await response.json();
        setEvents(data);
      } else {
        setError('Failed to fetch events');
      }
    } catch (err) {
      setError('Network error while fetching events');
    } finally {
      setLoading(false);
    }
  };

  const handleLogout = (): void => {
    logout();
  };

  const handleCreateEvent = (): void => {
    navigate('/events/create');
  };

  const formatEventDate = (dateString: string): string => {
    const date = new Date(dateString);
    return date.toLocaleDateString('en-US', {
      weekday: 'short',
      month: 'short',
      day: 'numeric',
      year: 'numeric'
    });
  };

  const getEventTypeColor = (eventType: number): string => {
    const colors: Record<number, string> = {
      1: 'rose', // Wedding
      2: 'pink', // Engagement
      3: 'blue', // Baptism
      4: 'indigo', // First Communion
      5: 'purple', // Quinceañera
      6: 'emerald', // Bar Mitzvah
      7: 'emerald', // Bat Mitzvah
      8: 'yellow', // Birthday
      9: 'rose', // Anniversary
      10: 'green', // Graduation
      11: 'green', // Baby Shower
      12: 'pink', // Bridal Shower
    };
    return colors[eventType] || 'gray';
  };

  const totalGuests = events.reduce((sum, event) => sum + event.totalGuests, 0);
  const totalRsvps = events.reduce((sum, event) => sum + event.confirmedRsvps, 0);

  return (
    <div className="min-h-screen bg-gray-50">
      {/* Header */}
      <header className="bg-white shadow-sm border-b">
        <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
          <div className="flex justify-between items-center py-4">
            <div className="flex items-center">
              <div className="bg-rose-100 p-2 rounded-lg mr-3">
                <Heart className="w-6 h-6 text-rose-600" />
              </div>
              <h1 className="text-2xl font-bold text-gray-900">Celebration Manager</h1>
            </div>
            
            <div className="flex items-center space-x-4">
              <span className="text-gray-700">
                Welcome, {user?.firstName || user?.name || user?.email}!
              </span>
              <button
                onClick={handleLogout}
                className="flex items-center px-3 py-2 text-gray-600 hover:text-gray-900 transition-colors"
              >
                <LogOut className="w-4 h-4 mr-2" />
                Logout
              </button>
            </div>
          </div>
        </div>
      </header>

      {/* Main Content */}
      <main className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
        {/* Welcome Section */}
        <div className="bg-gradient-to-r from-rose-500 to-pink-600 rounded-2xl p-8 text-white mb-8">
          <h2 className="text-3xl font-bold mb-2">Create Your Perfect Celebration</h2>
          <p className="text-rose-100 mb-6">
            Design beautiful invitations, manage your guest list, and track RSVPs for any special occasion.
          </p>
          <button 
            onClick={handleCreateEvent}
            className="bg-white text-rose-600 px-6 py-3 rounded-lg font-medium hover:bg-gray-50 transition-colors flex items-center"
          >
            <Plus className="w-5 h-5 mr-2" />
            Create New Event
          </button>
        </div>

        {/* Quick Stats */}
        <div className="grid grid-cols-1 md:grid-cols-3 gap-6 mb-8">
          <div className="bg-white rounded-xl p-6 shadow-sm border">
            <div className="flex items-center">
              <div className="bg-blue-100 p-3 rounded-lg mr-4">
                <Calendar className="w-6 h-6 text-blue-600" />
              </div>
              <div>
                <h3 className="text-lg font-semibold text-gray-900">{events.length} Events</h3>
                <p className="text-gray-600">Total events created</p>
              </div>
            </div>
          </div>

          <div className="bg-white rounded-xl p-6 shadow-sm border">
            <div className="flex items-center">
              <div className="bg-green-100 p-3 rounded-lg mr-4">
                <Users className="w-6 h-6 text-green-600" />
              </div>
              <div>
                <h3 className="text-lg font-semibold text-gray-900">{totalGuests} Guests</h3>
                <p className="text-gray-600">Total guests invited</p>
              </div>
            </div>
          </div>

          <div className="bg-white rounded-xl p-6 shadow-sm border">
            <div className="flex items-center">
              <div className="bg-purple-100 p-3 rounded-lg mr-4">
                <Heart className="w-6 h-6 text-purple-600" />
              </div>
              <div>
                <h3 className="text-lg font-semibold text-gray-900">{totalRsvps} RSVPs</h3>
                <p className="text-gray-600">Responses received</p>
              </div>
            </div>
          </div>
        </div>

        {/* Events List */}
        <div className="bg-white rounded-xl shadow-sm border">
          <div className="p-6 border-b border-gray-200 flex justify-between items-center">
            <h3 className="text-lg font-semibold text-gray-900">Your Events</h3>
            {events.length > 0 && (
              <button 
                onClick={handleCreateEvent}
                className="flex items-center px-4 py-2 bg-rose-600 text-white rounded-lg hover:bg-rose-700 transition-colors text-sm font-medium"
              >
                <Plus className="w-4 h-4 mr-2" />
                Add Event
              </button>
            )}
          </div>

          <div className="p-6">
            {loading ? (
              <div className="text-center py-12">
                <div className="w-8 h-8 border-4 border-rose-500 border-t-transparent rounded-full animate-spin mx-auto mb-4"></div>
                <p className="text-gray-600">Loading events...</p>
              </div>
            ) : error ? (
              <div className="text-center py-12">
                <div className="bg-red-100 p-4 rounded-full w-16 h-16 mx-auto mb-4 flex items-center justify-center">
                  <Calendar className="w-8 h-8 text-red-400" />
                </div>
                <h4 className="text-lg font-medium text-gray-900 mb-2">Error loading events</h4>
                <p className="text-gray-600 mb-4">{error}</p>
                <button 
                  onClick={fetchEvents}
                  className="text-rose-600 hover:text-rose-700 font-medium"
                >
                  Try again
                </button>
              </div>
            ) : events.length === 0 ? (
              <div className="text-center py-12">
                <div className="bg-gray-100 p-4 rounded-full w-16 h-16 mx-auto mb-4 flex items-center justify-center">
                  <Calendar className="w-8 h-8 text-gray-400" />
                </div>
                <h4 className="text-lg font-medium text-gray-900 mb-2">No events yet</h4>
                <p className="text-gray-600 mb-6">Create your first celebration event to get started.</p>
                <button 
                  onClick={handleCreateEvent}
                  className="bg-rose-600 text-white px-6 py-3 rounded-lg font-medium hover:bg-rose-700 transition-colors flex items-center mx-auto"
                >
                  <Plus className="w-5 h-5 mr-2" />
                  Create Your First Event
                </button>
              </div>
            ) : (
              <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
                {events.map((event) => {
                  const colorClass = getEventTypeColor(event.eventType);
                  const isUpcoming = event.isUpcoming;
                  
                  return (
                    <div key={event.id} className="border border-gray-200 rounded-lg p-6 hover:shadow-md transition-shadow cursor-pointer">
                      <div className="flex items-start justify-between mb-4">
                        <div className={`inline-flex px-3 py-1 rounded-full text-xs font-medium bg-${colorClass}-100 text-${colorClass}-700`}>
                          {event.eventTypeName}
                        </div>
                        {isUpcoming && event.daysUntilEvent <= 30 && (
                          <div className="bg-amber-100 text-amber-700 px-2 py-1 rounded-full text-xs font-medium">
                            {event.daysUntilEvent} days
                          </div>
                        )}
                      </div>

                      <h4 className="font-semibold text-gray-900 mb-2 text-lg">{event.title}</h4>
                      
                      <div className="space-y-2 mb-4">
                        <div className="flex items-center text-sm text-gray-600">
                          <Calendar className="w-4 h-4 mr-2" />
                          {formatEventDate(event.eventDate)}
                        </div>
                        <div className="flex items-center text-sm text-gray-600">
                          <MapPin className="w-4 h-4 mr-2" />
                          {event.location}
                        </div>
                      </div>

                      <div className="flex justify-between items-center pt-4 border-t border-gray-100">
                        <div className="text-sm text-gray-600">
                          <span className="font-medium">{event.totalGuests}</span> guests
                        </div>
                        <div className="text-sm text-gray-600">
                          <span className="font-medium text-green-600">{event.confirmedRsvps}</span> confirmed
                        </div>
                      </div>
                    </div>
                  );
                })}
              </div>
            )}
          </div>
        </div>
      </main>
    </div>
  );
}

export default Dashboard;