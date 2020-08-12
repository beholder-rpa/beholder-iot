import React from 'react';
import Loader from '../Loader';
import MessageView from '../MessageView'

interface InfoViewProps {

}

const InfoView: React.FC<InfoViewProps> = () => {
  //const { error, loading, message } = useSelector<AppState, AppState['common']>(({ common }) => common);
  const loading = false;
  const message = "hi";
  const error = "";

  const showMessage = () => {
    return <MessageView variant='success' message={message.toString()} />;
  };

  const showError = () => {
    return <MessageView variant='error' message={error.toString()} />;
  };

  return (
    <>
      {loading && <Loader />}

      {message && showMessage()}
      {error && showError()}
    </>
  );
};

export default InfoView;
