import { useState } from 'react';
import Register from './Register';
import Login from './Login';

function AuthPage({ onLoginSuccess }) {
    const [showRegister, setShowRegister] = useState(false);

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
                <Login onLoginSuccess={onLoginSuccess} onSwitchToRegister={() => setShowRegister(true)} />
            )}
        </div>
    );
}

export default AuthPage;