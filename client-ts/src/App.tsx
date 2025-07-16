import { BrowserRouter as Router, Routes, Route, Navigate } from 'react-router-dom';
import { AuthProvider } from './contexts/AuthContext';
import ProtectedRoute from './components/ProtectedRoute';
import AuthPage from './pages/AuthPage';
import Dashboard from './pages/Dashboard';
import EventCreationForm from './components/forms/EventCreationForm';
import EventDetailsPage from '../src/pages/EventsDetailsPage'; 
import PreviewPage from './pages/PreviewPage'; // Your existing preview
import './App.css';

function App() {
  return (
    <AuthProvider>
      <Router>
        <div className="App">
          <Routes>
            {/* Public Routes */}
            <Route path="/auth" element={<AuthPage />} />
            
            {/* Public Preview Route (for guests) */}
            <Route path="/preview/:id" element={<PreviewPage />} />
            
            {/* Protected Routes */}
            <Route path="/dashboard" element={
              <ProtectedRoute>
                <Dashboard />
              </ProtectedRoute>
            } />
            
            <Route path="/events/create" element={
              <ProtectedRoute>
                <EventCreationForm />
              </ProtectedRoute>
            } />
            
            {/* Event Management - Your new EventDetailsPage */}
            <Route path="/events/:id" element={
              <ProtectedRoute>
                <EventDetailsPage />
              </ProtectedRoute>
            } />
            
            {/* Default redirect */}
            <Route path="/" element={<Navigate to="/dashboard" replace />} />
            
            {/* Catch all - redirect to dashboard */}
            <Route path="*" element={<Navigate to="/dashboard" replace />} />
          </Routes>
        </div>
      </Router>
    </AuthProvider>
  );
}

export default App;