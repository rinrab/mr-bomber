#! /bin/bash

systemctl stop mr-bomber
git pull
dotnet publish -o /srv/mr-bomber
systemctl start mr-bomber

systemctl status mr-bomber

sleep 3s

systemctl status mr-bomber
