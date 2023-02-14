#FROM scratch
FROM centos:7
WORKDIR /home
COPY exec.sh /home/
COPY stools-linux-x64/* /home/
CMD [ "sh", "-c", "/home/exec.sh" ]