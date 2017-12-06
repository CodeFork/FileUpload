Very simple file upload and download ASP.NET Core application.

## Developer notes

Some notes to make development, build and publish easier.
Greate source of knowledge is at [Microsoft Docs](https://docs.microsoft.com/en-us/aspnet/core/publishing/linuxproduction).

### Publish to Raspberry

```
dotnet publish -c Release -r linux-arm
```

### Make ASP.NET Core application a service on Raspberry linux

Create a file at `/lib/systemd/system/{service_name}.service` (the extension '.service' is required) and paste a content:

```
[Unit]
Description={description} ASP.NET CORE app
After=multi-user.target

[Service]
Type=simple
ExecStart=/wwwroot/{path_to_executable}
Restart=on-abort

[Install]
WantedBy=multi-user.target
```

Than enable the new service and start it:

```
systemctl enable {service_name}

systemctl start {service_name}
```

After every publish and copy to server, you need to make executable executable:

```
chmod +x {path_to_executable}
```

### Show linux service logs

```
sudo journalctl -fu {service_name}.service
```

or 

```
sudo journalctl -fu {service_name}.service --since "2016-10-18" --until "2016-10-18 04:00"
```

### Register a website as a proxy for ASP.NET Core application in Nginx

Create a file in `/etc/nginx/sites-available/{website_name}` and paste a content:

```
server {
    listen       80;
    server_name  {domain1} {domain...};

    location / {
        proxy_pass  http://127.0.0.1:{kestrel_port};
    }
}
```

Note: Sometimes it may be important to use `127.0.0.1` instead of `localhost`.
To make website enabled, create a symbolic link in `sites-enabled`:

```
ln -s /etc/nginx/sites-available/{website_name} /etc/nginx/sites-enabled/{website_name}
```

and than restart Nginx.

```
nginx -s reload
```
