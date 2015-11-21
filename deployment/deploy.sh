#! /bin/sh

target=$1
ansible=$(which ansible-playbook)
playbookname="fruithap_engine_playbook.yml"
user=pi

if [ -z $target ] ; then
	echo "Usage: deploy_engine [target]" >&2
	exit 1
fi

if [ -z $ansible ] ; then
	echo "ERROR: Ansible should be installed" >&2
	exit 1
fi

if [ ! -f $playbookname ] ; then
	echo "ERROR: Installation playbook cannot be found. Maybe your release package is broken?" >&2
	exit 1
fi

$ansible $playbookname -u $user -s -vvvv -e target=$target

