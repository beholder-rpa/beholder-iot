import React, { ReactNode } from 'react';
import { useRouter } from 'next/router';
import Head from 'next/head';

const defaultTitle = 'Beholder RPA';
const defaultDescription =
  'Pure React hooks base admin template with material-ui.';
const defaultImage = `/images/logo-with-name.jpg`;
const defaultSep = ' | ';

interface BeholderPageProps {
  title?: string,
  description?: string,
  image?: string,
  contentType?: string,
  children: ReactNode
}


const BeholderPage: React.FC<BeholderPageProps> = (props) => {
  const { title, description, image, contentType, children } = props;
  const { pathname } = useRouter();


  const theTitle = title
    ? (title + defaultSep + defaultTitle).substring(0, 60)
    : defaultTitle;
  const theDescription = description
    ? description.substring(0, 155)
    : defaultDescription;
  const theImage = image ? `./${image}` : defaultImage;
  return (
    <>
      <Head>
        <title>
          {title ? title + defaultSep + defaultTitle : defaultTitle}
        </title>
        <meta property='name' content={theTitle} key='name' />
        <meta
          property='description'
          content={theDescription}
          key='description'
        />
        <meta property='og:title' content={theTitle} key='title' />
        <meta property='og:title' content={theTitle} key='title' />
        <meta property='og:type' content={contentType} key='type' />
        <meta property='og:url' content={pathname} key='url' />
        <meta property='og:image' content={theImage} key='v' />
        <meta
          property='og:description'
          content={theDescription}
          key='description'
        />
        <meta property='og:site_name' content={defaultTitle} key='site_name' />
      </Head>

      {children}
    </>
  );
};

export default BeholderPage;
