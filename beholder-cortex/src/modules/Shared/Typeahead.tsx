import React, { useState } from 'react';

type Client = {
  id: number;
  name: string;
};

const data: Client[] = [
  { id: 1, name: 'Toyota' },
  { id: 2, name: 'Santander' },
  { id: 3, name: 'GMF' },
  { id: 4, name: 'Acar Leasing' },
  { id: 5, name: 'AAcar Inc' },
];

const mockResults = (keyword): Promise<Client[]> => {
  return new Promise((res, _rej) => {
    setTimeout(() => {
      const searchResults = data.filter((item) => item.name.includes(keyword));
      res(searchResults);
    }, 500);
  });
};

export default function Typeahead() {
  const [results, setResults] = useState([]);
  const [isLoading, setIsLoading] = useState(false);
  const [name, setName] = useState('');
  const [isNameSelected, setIsNameSelected] = useState(false);

  const handleInputChange = (e: { target: { value: any } }) => {
    const nameValue = e.target.value;
    setName(nameValue);
    // even if we've selected already an item from the list, we should reset it since it's been changed
    setIsNameSelected(false);
    // clean previous results, as would be the case if we get the results from a server
    setResults([]);
    if (nameValue.length > 1) {
      setIsLoading(true);
      mockResults(nameValue)
        .then((res) => {
          setResults(res);
          setIsLoading(false);
        })
        .catch(() => {
          setIsLoading(false);
        });
    }
  };

  const onNameSelected = (selectedName) => {
    setName(selectedName);
    setIsNameSelected(true);
    setResults([]);
  };

  return (
    <div className="w-72">
      <div className="relative">
        <input className="form-field" type="text" autoComplete="off" onChange={handleInputChange} value={name} />
        <div className="absolute w-72 flex">
          <div className="bg-white shadow-xl rounded-lg w-72">
            <ul className="divide-y divide-gray-300">
              {!isNameSelected &&
                results.length > 0 &&
                results.map((result) => (
                  <li
                    key={result.id}
                    className="p-2 hover:bg-gray-50 cursor-pointer"
                    onClick={() => onNameSelected(result.name)}
                  >
                    {result.name}
                  </li>
                ))}
              {!results.length && isLoading && (
                <div className="flex justify-center items-center">
                  <div className="animate-spin rounded-full h-16 w-16 border-b-2 border-gray-900"></div>
                </div>
              )}
            </ul>
          </div>
        </div>
      </div>
    </div>
  );
}
