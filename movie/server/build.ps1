cd ../client
npm install
npm run build
cd ../server

docker build -t stools .
docker save -o ../stools.tar stools