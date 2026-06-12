import { NavLink, Outlet } from 'react-router-dom'
import { useAuth } from '../context/AuthContext'

export function Layout() {
  const { user, signOut } = useAuth()

  return (
    <div className="app-shell">
      <header className="app-header">
        <div className="header-inner">
          <div className="brand">
            <span className="brand-icon">🌤️</span>
            <span className="brand-name">MeuHumor</span>
          </div>

          <nav className="nav">
            <NavLink to="/" end className={({ isActive }) => (isActive ? 'nav-link active' : 'nav-link')}>
              Hoje
            </NavLink>
            <NavLink to="/historico" className={({ isActive }) => (isActive ? 'nav-link active' : 'nav-link')}>
              Histórico
            </NavLink>
          </nav>

          <div className="header-user">
            <span className="user-email">{user?.email}</span>
            <button type="button" className="btn btn-ghost" onClick={() => signOut()}>
              Sair
            </button>
          </div>
        </div>
      </header>

      <main className="app-main">
        <Outlet />
      </main>
    </div>
  )
}
