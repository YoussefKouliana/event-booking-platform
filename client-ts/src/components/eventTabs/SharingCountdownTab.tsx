import { useState, useEffect } from 'react';
import { CheckCircle, Copy, Mail, Share2, QrCode, Download } from 'lucide-react';

interface EventDetails {
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
}

export default function SharingCountdownTab({ event }: { event: EventDetails }) {
  const [copied, setCopied] = useState(false);
  const [daysLeft, setDaysLeft] = useState(0);
  const [hoursLeft, setHoursLeft] = useState(0);
  const [minutesLeft, setMinutesLeft] = useState(0);

  useEffect(() => {
    const updateCountdown = () => {
      const now = new Date();
      const eventDate = new Date(event.eventDate);
      const diff = eventDate.getTime() - now.getTime();
      if (diff > 0) {
        setDaysLeft(Math.floor(diff / (1000 * 60 * 60 * 24)));
        setHoursLeft(Math.floor((diff % (1000 * 60 * 60 * 24)) / (1000 * 60 * 60)));
        setMinutesLeft(Math.floor((diff % (1000 * 60 * 60)) / (1000 * 60)));
      }
    };

    updateCountdown();
    const interval = setInterval(updateCountdown, 60000);
    return () => clearInterval(interval);
  }, [event.eventDate]);

  const handleCopyLink = () => {
    const inviteLink = `${window.location.origin}/preview/${event.id}`;
    navigator.clipboard.writeText(inviteLink);
    setCopied(true);
    setTimeout(() => setCopied(false), 2000);
  };

  const shareToWhatsApp = () => {
    const message = `You're invited to ${event.title}! View it here: ${window.location.origin}/preview/${event.id}`;
    window.open(`https://wa.me/?text=${encodeURIComponent(message)}`, '_blank');
  };

  const shareViaEmail = () => {
    const subject = `You're Invited: ${event.title}`;
    const body = `You're invited to ${event.title}!\n\nDate: ${new Date(event.eventDate).toLocaleDateString()}\nLocation: ${event.location}\n\nView invitation: ${window.location.origin}/preview/${event.id}`;
    window.open(`mailto:?subject=${encodeURIComponent(subject)}&body=${encodeURIComponent(body)}`);
  };

  return (
    <div>
      {/* Countdown UI here */}
      {/* Keep your layout code unchanged here */}
    </div>
  );
}
