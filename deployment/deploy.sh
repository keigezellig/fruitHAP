#! /bin/sh

if [ $# -eq 2 ] ; then
	target=$1
	extraopts=$2
fi

if [ $# -eq 1 ] ; then
	extraopts=$1
fi

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
	cmdline="$ansible $playbookname -u $user -s $extraopts"
else
	cmdline="$ansible $playbookname -u $user -e target=$target -s $extraopts"
fi

$cmdline


