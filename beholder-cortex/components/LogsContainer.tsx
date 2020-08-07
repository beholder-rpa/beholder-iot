import React, { useState, useEffect } from 'react'
import { Console, Hook, Unhook } from 'console-feed';

export default () => {
  const [logs, setLogs] = useState([]);

  // run once!
  useEffect(() => {
    Hook(
      window.console,
      (log) => setLogs((currLogs) => [...currLogs, log]),
      false
    );
    return () => Unhook(window.console as any);
  }, []);

  return <Console logs={logs} variant="dark" />;
};
