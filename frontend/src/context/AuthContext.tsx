
import {
  createContext,
  useContext,
  useState,
  type ReactNode,
} from 'react';

interface AuthUser {
  username: string;
  role: string;
  expiresAt: string;
}

interface AuthContextType {
  user: AuthUser | null;
  token: string | null;
  login: (token: string, user: AuthUser) => void;
  logout: () => void;
}

const AuthContext = createContext<AuthContextType | undefined>(undefined);

export function AuthProvider({ children }: { children: ReactNode }) {
  const [token, setToken] = useState<string | null>(
    () => sessionStorage.getItem('token')
  );

  const [user, setUser] = useState<AuthUser | null>(() => {
    const savedUser = sessionStorage.getItem('user');

    if (!savedUser) return null;

    try {
      const parsedUser = JSON.parse(savedUser) as AuthUser;

      if (new Date(parsedUser.expiresAt).getTime() <= Date.now()) {
        sessionStorage.removeItem('token');
        sessionStorage.removeItem('user');
        return null;
      }

      return parsedUser;
    } catch {
      sessionStorage.removeItem('token');
      sessionStorage.removeItem('user');
      return null;
    }
  });

  function login(newToken: string, newUser: AuthUser) {
    sessionStorage.setItem('token', newToken);
    sessionStorage.setItem('user', JSON.stringify(newUser));

    setToken(newToken);
    setUser(newUser);
  }

  function logout() {
    sessionStorage.removeItem('token');
    sessionStorage.removeItem('user');

    setToken(null);
    setUser(null);
  }

  return (
    <AuthContext.Provider value={{ user, token, login, logout }}>
      {children}
    </AuthContext.Provider>
  );
}

export function useAuth() {
  const context = useContext(AuthContext);

  if (!context) {
    throw new Error('useAuth должен использоваться внутри AuthProvider.');
  }

  return context;
}
