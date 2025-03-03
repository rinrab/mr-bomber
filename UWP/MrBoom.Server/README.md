

```sh
sudo yum install git dotnet

git clone https://github.com/rinrab/mr-bomber
### git switch multiplayer2
cd ./mr-bomber/UWP/MrBoom.Server

dotnet build

sudo mkdir /srv/mr-bomber
sudo dotnet publish -o /srv/mr-bomber

sudo setcap CAP_NET_BIND_SERVICE=+eip /srv/mr-bomber/MrBoom.Server

/srv/mr-bomber/MrBoom.Server

sudo mkdir /etc/systemd/system/ -p
sudo cp mr-bomber.service /etc/systemd/system/mr-bomber.service
sudo systemctl daemon-reload

sudo systemctl start mr-bomber
systemctl status mr-bomber

sudo systemctl stop mr-bomber
sudo dotnet publish -o /srv/mr-bomber
sudo systemctl start mr-bomber
systemctl status mr-bomber
```
