import { useEffect, useState } from 'react';
import './App.css';
import Dashboard from './components/Dashboard';
import AuthPage from './components/AuthPage';

function App() {
    const [user, setUser] = useState(null);
    // Add a loading state to prevent flickering
    const [isLoading, setIsLoading] = useState(true);

    const checkAuthStatus = async () => {
        try {
            const response = await fetch('/api/user', {
                credentials: 'include'
            });

            if (response.ok) {
                const userData = await response.json();
                setUser(userData);
            }
        } catch (error) {
            console.error('Error checking auth status:', error);
        } finally {
            // Stop loading once the check is complete
            setIsLoading(false);
        }
    };

    useEffect(() => {
        checkAuthStatus();
    }, []);

    const handleLoginSuccess = (userData) => {
        setUser(userData);
    };

    const handleLogout = async () => {
        try {
            await fetch('/api/auth/logout', {
                method: 'POST',
                credentials: 'include'
            });
        } catch (error) {
            console.error('Error during logout:', error);
        }
        setUser(null);
    };

    // Show a loading indicator while we check auth status
    if (isLoading) {
        return (
            <div style={{
                display: 'flex',
                justifyContent: 'center',
                alignItems: 'center',
                minHeight: '100vh',
                fontSize: '1.5rem',
                color: '#666'
            }}>
                Loading...
            </div>
        );
    }

    return (
        <div>
            {user ? (
                <Dashboard user={user} onLogout={handleLogout} />
            ) : (
                <AuthPage onLoginSuccess={handleLoginSuccess} />
            )}
        </div>
    );
}

export default App;