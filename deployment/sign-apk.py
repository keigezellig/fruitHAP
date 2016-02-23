# !/usr/bin/python
import argparse
import subprocess
import sys

parser = argparse.ArgumentParser()
parser.add_argument("input", help="Full path to apk file")
parser.add_argument("output", help="Output apk")
parser.add_argument("keystore", help="Full path to keystore")
parser.add_argument("keyalias", help="Key alias")
parser.add_argument("storepass", help="Keystore password")
parser.add_argument("keypass", help="Key password")
parser.add_argument("pathtojarsigner")
parser.add_argument("pathtozipalign")

args = parser.parse_args()
exitcode = 0;
print args

try:
    print "**** Sign apk *****"
    subprocess.check_call(
        [args.pathtojarsigner, '-verbose', '-sigalg', 'SHA1withRSA', '-digestalg', 'SHA1', '-storepass', args.storepass, '-keystore',
        args.keystore, '-keypass', args.keypass, '-signedjar', 'signed.apk', args.input, args.keyalias])

    print "**** Zip align *****"
    subprocess.check_call(
        [args.pathtozipalign, '-v', '4', 'signed.apk', args.output])
except subprocess.CalledProcessError as ex:
    exitcode = ex.returncode


print exitcode
sys.exit(exitcode)

