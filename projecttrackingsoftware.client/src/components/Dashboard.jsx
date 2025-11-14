// File: projecttrackingsoftware.client/src/components/Dashboard.jsx

import React, { useState, useEffect } from 'react';
import CreateProjectModal from './CreateProjectModal'; // Import the new component

function Dashboard({ user, onLogout }) {
    const [projects, setProjects] = useState([]);
    const [isProjectsLoading, setIsProjectsLoading] = useState(true);
    const [showCreateModal, setShowCreateModal] = useState(false);

    // DTO Interface for reference
    // { id, title, description, color, ownerName, userRole, createdAt }

    // Function to fetch the list of user projects (GET /api/projects)
    const fetchProjects = async () => {
        setIsProjectsLoading(true);
        try {
            const response = await fetch('/api/projects', {
                credentials: 'include'
            });

            if (response.ok) {
                const data = await response.json();
                setProjects(data);
            } else {
                console.error('Failed to fetch projects:', response.statusText);
            }
        } catch (error) {
            console.error('Error fetching projects:', error);
        } finally {
            setIsProjectsLoading(false);
        }
    };

    // Load projects on component mount
    useEffect(() => {
        fetchProjects();
    }, []);

    // Handler to refresh the list after a new project is created
    const handleProjectCreated = (newProject) => {
        // Optimistically add the new project to the list without a full refetch
        setProjects(prev => [newProject, ...prev]);
        // Alternatively, you could call: fetchProjects();
    }

    // Project Card Renderer
    const ProjectCard = ({ project }) => {
        const dateCreated = new Date(project.createdAt).toLocaleDateString();

        return (
            <div
                key={project.id}
                style={{
                    backgroundColor: 'white',
                    padding: '1.5rem',
                    borderRadius: '8px',
                    boxShadow: '0 2px 10px rgba(0,0,0,0.1)',
                    borderLeft: `5px solid ${project.color}`, // Project color on the left
                    cursor: 'pointer',
                    transition: 'transform 0.2s',
                    // Note: You will later implement routing to /project/{project.id} here
                }}
                // Placeholder for future navigation to the Project Details
                onClick={() => alert(`Navigating to Project: ${project.title}`)}
            >
                <h4 style={{
                    color: '#2c3e50',
                    marginTop: 0,
                    marginBottom: '0.5rem',
                    fontSize: '1.25rem'
                }}>
                    {project.title}
                </h4>
                <p style={{
                    fontSize: '0.9rem',
                    color: '#666',
                    marginBottom: '1rem',
                    maxHeight: '40px',
                    overflow: 'hidden',
                    textOverflow: 'ellipsis'
                }}>
                    {project.description || 'No description provided.'}
                </p>
                <div style={{ fontSize: '0.85rem', color: '#888' }}>
                    <p style={{ margin: '0.25rem 0' }}>
                        **Role:** <span style={{ fontWeight: 'bold', color: project.userRole === 'Owner' ? '#c0392b' : '#34495e' }}>{project.userRole}</span>
                    </p>
                    <p style={{ margin: '0.25rem 0' }}>
                        Owner: {project.ownerName}
                    </p>
                    <p style={{ margin: '0.25rem 0' }}>
                        Created: {dateCreated}
                    </p>
                </div>
            </div>
        );
    };


    return (
        <div style={{ minHeight: '100vh', backgroundColor: '#f8f9fa' }}>
            {/* Render Modal if state is true */}
            {showCreateModal && (
                <CreateProjectModal
                    onClose={() => setShowCreateModal(false)}
                    onProjectCreated={handleProjectCreated}
                />
            )}

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
                    {/* Logout Button in Header */}
                    <button
                        onClick={onLogout}
                        style={{
                            padding: '0.5rem 1rem',
                            backgroundColor: '#dc3545',
                            color: 'white',
                            border: 'none',
                            borderRadius: '4px',
                            cursor: 'pointer'
                        }}>
                        Logout
                    </button>
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
                        Hello, {user.username || user.email.split('@')[0]}!
                    </h2>
                    <p style={{
                        fontSize: '1.2rem',
                        margin: 0,
                        opacity: 0.9
                    }}>
                        Welcome back to your Project Tracking Dashboard
                    </p>
                </div>

                {/* Dashboard Cards and Project List */}
                <div style={{
                    display: 'grid',
                    gridTemplateColumns: 'repeat(auto-fit, minmax(300px, 1fr))',
                    gap: '1.5rem',
                    marginTop: '2rem'
                }}>

                    {/* Quick Actions Card (Modified) */}
                    <div style={{
                        backgroundColor: 'white',
                        padding: '2rem',
                        borderRadius: '8px',
                        boxShadow: '0 2px 10px rgba(0,0,0,0.1)',
                        gridColumn: '1 / -1' // Make quick actions span the full width if space allows
                    }}>
                        <h3 style={{ color: '#2c3e50', marginTop: 0 }}>Project Actions</h3>
                        <button
                            onClick={() => setShowCreateModal(true)} // Open the modal
                            style={{
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
                    </div>

                    {/* Projects List Section */}
                    <div style={{ gridColumn: '1 / -1' }}>
                        <h3 style={{ color: '#2c3e50', marginTop: 0 }}>Your Projects</h3>

                        {isProjectsLoading && (
                            <p style={{ textAlign: 'center', padding: '2rem', color: '#666' }}>Loading projects...</p>
                        )}

                        {!isProjectsLoading && projects.length === 0 && (
                            <div style={{ padding: '2rem', textAlign: 'center', backgroundColor: 'white', borderRadius: '8px', boxShadow: '0 2px 10px rgba(0,0,0,0.1)' }}>
                                <p style={{ fontSize: '1.1rem', color: '#888' }}>
                                    You are not a member of any projects. Start by creating a new one!
                                </p>
                                <button
                                    onClick={() => setShowCreateModal(true)}
                                    style={{
                                        padding: '0.75rem 1.5rem',
                                        backgroundColor: '#3498db',
                                        color: 'white',
                                        border: 'none',
                                        borderRadius: '4px',
                                        cursor: 'pointer',
                                        marginTop: '1rem'
                                    }}>
                                    Create First Project
                                </button>
                            </div>
                        )}

                        {!isProjectsLoading && projects.length > 0 && (
                            <div style={{
                                display: 'grid',
                                gridTemplateColumns: 'repeat(auto-fill, minmax(300px, 1fr))',
                                gap: '1.5rem'
                            }}>
                                {projects.map(project => (
                                    <ProjectCard key={project.id} project={project} />
                                ))}
                            </div>
                        )}
                    </div>
                </div>
            </main>
        </div>
    );
}

export default Dashboard;