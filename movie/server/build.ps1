cd ../client
npm install
npm run build
cd ../server

docker build -t movie .
docker save -o ../movie.tar movie