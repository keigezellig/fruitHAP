#! /bin/sh

target=$1
ansible=$(which ansible-playbook)
playbookname="fruithap_engine_playbook.yml"
user=pi

if [ -z $ansible ] ; then
	echo "ERROR: Ansible should be installed" >&2
	exit 1
fi

if [ ! -f $playbookname ] ; then
	echo "ERROR: Installation playbook cannot be found. Maybe your release package is broken?" >&2
	exit 1
fi

if [ -z $target ] ; then
	cmdline="$ansible $playbookname -u $user -s"
else
	cmdline="$ansible $playbookname -u $user -s -e target=$target"
fi

echo $cmdline
$cmdline


