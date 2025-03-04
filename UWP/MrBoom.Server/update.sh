#! /bin/bash

systemctl stop mr-bomber
git pull
dotnet publish -o /srv/mr-bomber
systemctl start mr-bomber

systemctl status mr-bomber

sleep 1s

systemctl status mr-bomber
