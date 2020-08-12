import React, { ReactNode } from 'react';
import Link from 'next/link';

interface AppNavLinkProps {
  to: string;
  children: ReactNode;

  [x: string]: any
}

const AppNavLink: React.FC<AppNavLinkProps> = ({ to, children, ...rest }) => (
  <Link href={to} {...rest}>
    <>{children}</>
  </Link>
);

export default AppNavLink;
