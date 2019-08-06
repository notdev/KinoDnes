docker rm dd-agent -f
DOCKER_CONTENT_TRUST=1 \
docker run -d --name dd-agent \
              -v /var/run/docker.sock:/var/run/docker.sock:ro \
              -v /proc/:/host/proc/:ro \
              -v /sys/fs/cgroup/:/host/sys/fs/cgroup:ro \
              -v /opt/datadog-agent/conf.d:/conf.d:ro \
              -v /opt/datadog-agent/checks.d:/checks.d:ro \
              -e DD_API_KEY=<API_KEY> \
              datadog/agent:latest
