import fs from 'fs';
import path from 'path';

import type { NextApiRequest, NextApiResponse } from 'next';

const filePath = path.resolve('/srv/app/certs', './server.crt');
const certBuffer = fs.readFileSync(filePath);

export default function handler(req: NextApiRequest, res: NextApiResponse) {
  // Get the certificate from /certs/server.crt and return it to the client
  res.setHeader('Content-Type', 'application/x-x509-ca-cert');
  res.send(certBuffer);
}
