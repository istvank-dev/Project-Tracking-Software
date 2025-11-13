import { useState } from 'react';

function Login({ onLoginSuccess, onSwitchToRegister }) {
    const [email, setEmail] = useState('');
    const [password, setPassword] = useState('');
    const [isLoading, setIsLoading] = useState(false);

    const handleSubmit = async (e) => {
        e.preventDefault();
        setIsLoading(true);

        try {
            const response = await fetch('/api/auth/login', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                },
                body: JSON.stringify({ email, password }),
                credentials: 'include'
            });

            if (response.ok) {
                console.log('Login successful');

                // Try to get user info - retry a few times if needed
                let userData = null;
                let retries = 3;

                while (retries > 0 && !userData) {
                    try {
                        const userResponse = await fetch('/api/user', {
                            credentials: 'include'
                        });

                        if (userResponse.ok) {
                            userData = await userResponse.json();
                        } else {
                            retries--;
                            await new Promise(resolve => setTimeout(resolve, 500));
                        }
                    } catch (error) {
                        retries--;
                        await new Promise(resolve => setTimeout(resolve, 500));
                    }
                }

                if (userData) {
                    onLoginSuccess(userData);
                } else {
                    // If we still can't get user info, at least show the email
                    onLoginSuccess({ username: email.split('@')[0], email: email });
                }
            } else {
                const errorData = await response.json();
                alert(`Login failed: ${errorData.message || 'Invalid credentials'}`);
            }
        } catch (error) {
            console.error('Error during login:', error);
            alert('An error occurred during login.');
        } finally {
            setIsLoading(false);
        }
    };

    return (
        <div style={{ maxWidth: '400px', margin: '0 auto' }}>
            <h2>Login</h2>
            <form onSubmit={handleSubmit}>
                <div style={{ marginBottom: '1rem' }}>
                    <label style={{ display: 'block', marginBottom: '0.5rem' }}>Email:</label>
                    <input
                        type="email"
                        value={email}
                        onChange={(e) => setEmail(e.target.value)}
                        required
                        style={{ width: '100%', padding: '0.5rem' }}
                    />
                </div>
                <div style={{ marginBottom: '1rem' }}>
                    <label style={{ display: 'block', marginBottom: '0.5rem' }}>Password:</label>
                    <input
                        type="password"
                        value={password}
                        onChange={(e) => setPassword(e.target.value)}
                        required
                        style={{ width: '100%', padding: '0.5rem' }}
                    />
                </div>
                <button
                    type="submit"
                    disabled={isLoading}
                    style={{
                        width: '100%',
                        padding: '0.75rem',
                        backgroundColor: isLoading ? '#ccc' : '#007bff',
                        color: 'white',
                        border: 'none',
                        borderRadius: '4px'
                    }}
                >
                    {isLoading ? 'Logging in...' : 'Login'}
                </button>
            </form>
            <p style={{ textAlign: 'center', marginTop: '1rem' }}>
                Don't have an account?{' '}
                <button
                    onClick={onSwitchToRegister}
                    style={{
                        background: 'none',
                        border: 'none',
                        color: '#007bff',
                        cursor: 'pointer',
                        textDecoration: 'underline'
                    }}
                >
                    Register here
                </button>
            </p>
        </div>
    );
}

export default Login;