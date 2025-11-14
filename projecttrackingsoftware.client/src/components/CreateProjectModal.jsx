// File: projecttrackingsoftware.client/src/components/CreateProjectModal.jsx

import React, { useState } from 'react';

// Default project color options
const defaultColors = [
    { name: 'Blue', hex: '#3498db' },
    { name: 'Green', hex: '#2ecc71' },
    { name: 'Orange', hex: '#e67e22' },
    { name: 'Red', hex: '#e74c3c' },
];

function CreateProjectModal({ onClose, onProjectCreated }) {
    const [title, setTitle] = useState('');
    const [description, setDescription] = useState('');
    const [color, setColor] = useState(defaultColors[0].hex);
    const [isLoading, setIsLoading] = useState(false);

    const handleSubmit = async (e) => {
        e.preventDefault();
        setIsLoading(true);

        const projectData = {
            title,
            description,
            color,
        };

        try {
            const response = await fetch('/api/projects', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                },
                body: JSON.stringify(projectData),
                credentials: 'include',
            });

            if (response.ok) {
                const newProject = await response.json();
                alert(`Project '${newProject.title}' created successfully!`);
                onProjectCreated(newProject); // Notify dashboard to refresh list
                onClose(); // Close the modal
            } else {
                const errorData = await response.json();
                alert(`Project creation failed: ${errorData.message || 'Server error.'}`);
            }
        } catch (error) {
            console.error('Error creating project:', error);
            alert('An unexpected error occurred.');
        } finally {
            setIsLoading(false);
        }
    };

    return (
        <div style={modalOverlayStyle}>
            <div style={modalContentStyle}>
                <h3 style={{ borderBottom: '1px solid #ddd', paddingBottom: '1rem', marginBottom: '1.5rem', color: '#2c3e50' }}>
                    Create New Project
                </h3>
                <form onSubmit={handleSubmit}>
                    {/* Title Input */}
                    <div style={formGroupStyle}>
                        <label style={labelStyle}>Title</label>
                        <input
                            type="text"
                            value={title}
                            onChange={(e) => setTitle(e.target.value)}
                            required
                            style={inputStyle}
                        />
                    </div>

                    {/* Description Input */}
                    <div style={formGroupStyle}>
                        <label style={labelStyle}>Description (Optional)</label>
                        <textarea
                            value={description}
                            onChange={(e) => setDescription(e.target.value)}
                            style={{ ...inputStyle, minHeight: '80px' }}
                        />
                    </div>

                    {/* Color Picker/Selector */}
                    <div style={formGroupStyle}>
                        <label style={labelStyle}>Project Color</label>
                        <div style={{ display: 'flex', gap: '10px' }}>
                            {defaultColors.map((c) => (
                                <div
                                    key={c.hex}
                                    onClick={() => setColor(c.hex)}
                                    title={c.name}
                                    style={{
                                        width: '30px',
                                        height: '30px',
                                        borderRadius: '50%',
                                        backgroundColor: c.hex,
                                        cursor: 'pointer',
                                        border: color === c.hex ? '3px solid #2c3e50' : '2px solid #ccc',
                                        transition: 'border 0.2s',
                                    }}
                                />
                            ))}
                        </div>
                    </div>

                    {/* Action Buttons */}
                    <div style={{ display: 'flex', justifyContent: 'flex-end', gap: '1rem', marginTop: '2rem' }}>
                        <button
                            type="button"
                            onClick={onClose}
                            disabled={isLoading}
                            style={{ padding: '0.75rem 1.5rem', backgroundColor: '#ccc', color: '#333', border: 'none', borderRadius: '4px' }}
                        >
                            Cancel
                        </button>
                        <button
                            type="submit"
                            disabled={isLoading}
                            style={{
                                padding: '0.75rem 1.5rem',
                                backgroundColor: isLoading ? '#ccc' : color,
                                color: 'white',
                                border: 'none',
                                borderRadius: '4px'
                            }}
                        >
                            {isLoading ? 'Creating...' : 'Create Project'}
                        </button>
                    </div>
                </form>
            </div>
        </div>
    );
}

export default CreateProjectModal;

// --- Minimal Styling for the Modal ---
const modalOverlayStyle = {
    position: 'fixed',
    top: 0,
    left: 0,
    right: 0,
    bottom: 0,
    backgroundColor: 'rgba(0, 0, 0, 0.5)',
    display: 'flex',
    justifyContent: 'center',
    alignItems: 'center',
    zIndex: 1000,
};

const modalContentStyle = {
    backgroundColor: 'white',
    padding: '2rem',
    borderRadius: '8px',
    width: '90%',
    maxWidth: '500px',
    boxShadow: '0 4px 15px rgba(0, 0, 0, 0.2)',
};

const formGroupStyle = {
    marginBottom: '1rem',
};

const labelStyle = {
    display: 'block',
    marginBottom: '0.5rem',
    fontWeight: '600',
    color: '#333',
};

const inputStyle = {
    width: '100%',
    padding: '0.75rem',
    boxSizing: 'border-box',
    border: '1px solid #ddd',
    borderRadius: '4px',
};