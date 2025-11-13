import { useEffect, useState } from 'react';
import './App.css';
import Register from './components/Register';
import Login from './components/Login';

function App() {
    const [user, setUser] = useState(null);
    const [showRegister, setShowRegister] = useState(false);

    // Define checkAuthStatus BEFORE useEffect
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

    if (user) {
        return (
            <div style={{ minHeight: '100vh', backgroundColor: '#f8f9fa' }}>
                {/* Header with Navigation */}
                <header style={{
                    backgroundColor: 'white',
                    boxShadow: '0 2px 4px rgba(0,0,0,0.1)',
                    padding: '1rem 0',
                    marginBottom: '2rem'
                }}>
                    <div style={{
                        maxWidth: '1200px',
                        margin: '0 auto',
                        padding: '0 2rem',
                        display: 'flex',
                        justifyContent: 'space-between',
                        alignItems: 'center'
                    }}>
                        <h1 style={{
                            margin: 0,
                            color: '#2c3e50',
                            fontSize: '1.8rem',
                            fontWeight: '600'
                        }}>
                            Project Tracking Software
                        </h1>
                        <div style={{ display: 'flex', alignItems: 'center', gap: '1rem' }}>
                            <span style={{
                                color: '#666',
                                fontSize: '0.9rem'
                            }}>
                                Welcome, <strong>{user.username || user.email}</strong>
                            </span>
                            <button
                                onClick={handleLogout}
                                style={{
                                    padding: '0.5rem 1rem',
                                    backgroundColor: '#dc3545',
                                    color: 'white',
                                    border: 'none',
                                    borderRadius: '4px',
                                    cursor: 'pointer',
                                    fontSize: '0.9rem'
                                }}
                            >
                                Logout
                            </button>
                        </div>
                    </div>
                </header>

                {/* Main Content */}
                <main style={{
                    maxWidth: '1200px',
                    margin: '0 auto',
                    padding: '0 2rem'
                }}>
                    <div style={{
                        background: 'linear-gradient(135deg, #667eea 0%, #764ba2 100%)',
                        color: 'white',
                        padding: '3rem 2rem',
                        borderRadius: '12px',
                        textAlign: 'center',
                        marginBottom: '2rem'
                    }}>
                        <h2 style={{
                            fontSize: '2.5rem',
                            margin: '0 0 1rem 0',
                            fontWeight: '700'
                        }}>
                            Hello, {user.username || user.email.split('@')[0]}! 👋
                        </h2>
                        <p style={{
                            fontSize: '1.2rem',
                            margin: 0,
                            opacity: 0.9
                        }}>
                            Welcome back to your Project Tracking Dashboard
                        </p>
                    </div>

                    {/* Dashboard Cards */}
                    <div style={{
                        display: 'grid',
                        gridTemplateColumns: 'repeat(auto-fit, minmax(300px, 1fr))',
                        gap: '1.5rem',
                        marginTop: '2rem'
                    }}>
                        <div style={{
                            backgroundColor: 'white',
                            padding: '2rem',
                            borderRadius: '8px',
                            boxShadow: '0 2px 10px rgba(0,0,0,0.1)'
                        }}>
                            <h3 style={{ color: '#2c3e50', marginTop: 0 }}>Your Profile</h3>
                            <div style={{ marginTop: '1rem' }}>
                                <p><strong>Username:</strong> {user.username || 'Not set'}</p>
                                <p><strong>Email:</strong> {user.email}</p>
                            </div>
                        </div>

                        <div style={{
                            backgroundColor: 'white',
                            padding: '2rem',
                            borderRadius: '8px',
                            boxShadow: '0 2px 10px rgba(0,0,0,0.1)'
                        }}>
                            <h3 style={{ color: '#2c3e50', marginTop: 0 }}>Quick Actions</h3>
                            <div style={{ marginTop: '1rem' }}>
                                <button style={{
                                    width: '100%',
                                    padding: '0.75rem',
                                    marginBottom: '0.5rem',
                                    backgroundColor: '#28a745',
                                    color: 'white',
                                    border: 'none',
                                    borderRadius: '4px',
                                    cursor: 'pointer'
                                }}>
                                    Create New Project
                                </button>
                                <button style={{
                                    width: '100%',
                                    padding: '0.75rem',
                                    backgroundColor: '#17a2b8',
                                    color: 'white',
                                    border: 'none',
                                    borderRadius: '4px',
                                    cursor: 'pointer'
                                }}>
                                    View Dashboard
                                </button>
                            </div>
                        </div>

                        <div style={{
                            backgroundColor: 'white',
                            padding: '2rem',
                            borderRadius: '8px',
                            boxShadow: '0 2px 10px rgba(0,0,0,0.1)'
                        }}>
                            <h3 style={{ color: '#2c3e50', marginTop: 0 }}>Recent Activity</h3>
                            <div style={{ marginTop: '1rem', color: '#666' }}>
                                <p>No recent activity</p>
                                <p style={{ fontSize: '0.9rem' }}>
                                    Start by creating your first project!
                                </p>
                            </div>
                        </div>
                    </div>
                </main>
            </div>
        );
    }

    return (
        <div>
            <h1>Project Tracking Software</h1>
            <p>Welcome! Please register or login to continue.</p>

            <div style={{ marginBottom: '1rem' }}>
                <button
                    onClick={() => setShowRegister(false)}
                    style={{
                        marginRight: '1rem',
                        backgroundColor: showRegister ? '#f0f0f0' : '#007bff',
                        color: showRegister ? '#333' : 'white'
                    }}
                >
                    Login
                </button>
                <button
                    onClick={() => setShowRegister(true)}
                    style={{
                        backgroundColor: showRegister ? '#007bff' : '#f0f0f0',
                        color: showRegister ? 'white' : '#333'
                    }}
                >
                    Register
                </button>
            </div>

            {showRegister ? (
                <Register onSwitchToLogin={() => setShowRegister(false)} />
            ) : (
                <Login onLoginSuccess={handleLoginSuccess} onSwitchToRegister={() => setShowRegister(true)} />
            )}
        </div>
    );
}

export default App;