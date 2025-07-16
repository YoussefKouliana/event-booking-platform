import { useState, useEffect } from 'react';
import { Users, Download, Plus, Upload, CheckCircle, XCircle, Clock } from 'lucide-react';

interface Guest {
  id: number;
  name: string;
  email: string;
  rsvpStatus: 'Pending' | 'Attending' | 'Declined';
  tableNumber?: string;
}

export default function RsvpManagementTab({ eventId }: { eventId: number }) {
  const [guests, setGuests] = useState<Guest[]>([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    fetchGuests();
  }, [eventId]);

  const fetchGuests = async () => {
    try {
      const response = await fetch(`/api/events/${eventId}/guests`, {
        headers: { 'Authorization': `Bearer ${localStorage.getItem('token')}` }
      });
      if (response.ok) {
        const data = await response.json();
        setGuests(data);
      }
    } catch (err) {
      console.error('Failed to fetch guests:', err);
    } finally {
      setLoading(false);
    }
  };

  const confirmed = guests.filter(g => g.rsvpStatus === 'Attending').length;
  const pending = guests.filter(g => g.rsvpStatus === 'Pending').length;
  const declined = guests.filter(g => g.rsvpStatus === 'Declined').length;

  return (
    <div>
      {/* RSVP UI here */}
      {/* Keep your layout code unchanged here */}
    </div>
  );
}
