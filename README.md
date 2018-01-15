# student-managment-system

ensure python and openssh-server are installed on the target vm,
On the host machine ensure `sudo apt install python-pip` and `pip install docker` have been done to install the Docker SDK for executing the ansible docker module.

Invoke ansible playbook installs docker:
```bash
ansible-playbook main.yml -i inventory.yml --become --ask-become-pass --ask-vault-pass --ask-pass --user username
```

ensure user is in docker group
```bash
usermod -aG docker username
```

ssh into the vm to start a mssql docker container:
```bash
docker run -e 'ACCEPT_EULA=Y' -e 'MSSQL_SA_PASSWORD=Abcd!12345' -p 1400:1433 --name sql1 -d microsoft/mssql-server-linux:2017-latest
```

set the container to restart
```bash
docker update --restart=unless-stopped containerIdNum
docker update --restart=unless-stopped $(docker ps -aqf "name=sql1")
```

install mssql tools
```bash
curl https://packages.microsoft.com/keys/microsoft.asc | sudo apt-key add -

curl https://packages.microsoft.com/config/ubuntu/16.04/prod.list | sudo tee /etc/apt/sources.list.d/msprod.list

sudo apt-get update 
sudo apt-get install mssql-tools unixodbc-dev

echo 'export PATH="$PATH:/opt/mssql-tools/bin"' >> ~/.bashrc
source ~/.bashrc
```
stop the docker container
```bash
docker stop sql1
```

remove the docker container
```bash 
docker rm sql1
```
get the container ID by name
``` bash
sudo docker ps -aqf "name=sql1"
```
instance ip address 
```bash
docker inspect --format='{{range .NetworkSettings.Networks}}{{.IPAddress}}{{end}}' $(docker ps -aqf "name=sql1")
```

port bindings
```bash
docker inspect --format='{{range $p, $conf := .NetworkSettings.Ports}} {{$p}} -> {{(index $conf 0).HostPort}} {{end}}' $(docker ps -aqf "name=sql1")
```