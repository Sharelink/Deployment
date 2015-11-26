#init server
groupadd deployment
useradd deployment -m -d /home/deployment -g deployment

#set mono env
echo '#set mono thread env’>>/etc/profile
echo 'export MONO_THREADS_PER_CPU=2000'>>/etc/profile
