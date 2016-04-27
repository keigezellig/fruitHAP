# update-version <major.minor.patch> <build_type> <build_no> <type> <rootdir>
import argparse
import re
import fnmatch
import os


def createArgumentParser():
    parser = argparse.ArgumentParser()
    parser.add_argument("rootdir", help='Project rootdir')
    parser.add_argument("type", help='Project type. N=.NET, A=Android', choices=['N', 'A'])
    parser.add_argument("--version", help='Version string in the following format [major.minor.patch]')
    parser.add_argument("--build_type", help='Optional build type', choices=['dev', 'rc', 'release'], nargs='?',
                        default='dev')
    parser.add_argument("--build_no", help='Optional build number')

    return parser


def get_existing_version(lines):
    regex = re.compile('.*(\d\.\d\.\d).*')
    list = [match.group(1) for line in lines for match in [regex.search(line)] if match]
    return list[2]


def processDotNet(rootdir, version_string, build_type, build_no):
    filetolookfor = 'AssemblyInfo.cs'
    for root, dirnames, filenames in os.walk(rootdir):
        for filename in fnmatch.filter(filenames, filetolookfor):
            filetobeprocessed = os.path.join(root, filename)
            print "Processing file: " + filetobeprocessed
            findAndReplaceDotNetVersionString(filetobeprocessed, version_string, build_type, build_no)


def findAndReplaceDotNetVersionString(filename, version_string, build_type, build_no):
    actual_versionstring = None
    assembly_versionstring = None
    semantic_version = None

    inputfile = open(filename)
    lines = inputfile.readlines()
    outputlines = []
    inputfile.close()

    if version_string is not None:
        actual_versionstring = version_string
    else:
        actual_versionstring = get_existing_version(lines)



    if build_no is not None:
        assembly_versionstring = actual_versionstring + '.' + build_no
    else:
        assembly_versionstring = actual_versionstring + '.0'

    if build_type == 'release':
        build_prefix = None
    else:
        build_prefix = build_type

    semantic_version = createSemanticVersionString(actual_versionstring, build_prefix, build_no)

    for inputline in lines:

        if inputline.startswith('[assembly: AssemblyVersion'):
            outputline = '[assembly: AssemblyVersion("' + assembly_versionstring + '")]\n'
            outputlines.append(outputline)
        elif inputline.startswith('[assembly: AssemblyFileVersion'):
            outputline = '[assembly: AssemblyFileVersion("' + assembly_versionstring + '")]\n'
            outputlines.append(outputline)
        elif inputline.startswith('[assembly: AssemblyInformationalVersion'):
            outputline = '[assembly: AssemblyInformationalVersion("' + semantic_version + '")]\n'
            outputlines.append(outputline)
        else:
            outputlines.append(inputline)

    outputfile = open(filename, 'w')
    outputfile.writelines(outputlines)


def createSemanticVersionString(version_no, build_prefix, build_no):
    #    0.0.1
    #    1.0.0
    #    1.1.2
    #    1.0.0-alpha
    #    1.0.0-rc.2
    #    1.0.0+build.3.ea34fd5c
    #    1.0.0-beta.3+build.20.f42cbd56

    if version_no == "":
        raise ValueError("Version number cannot be empty")

    version_pattern = re.compile("\d\.\d\.\d")  # version number should be 3 digits separated by a period
    if not re.match(version_pattern, version_no):
        raise ValueError("Version number should be in the format x.x.x where x is a digit from 0-9")

    result = version_no

    if build_prefix is not None:
        result += "-" + build_prefix

    if build_prefix is not None and build_no is not None:
        result += "+build." + build_no

    return result


if __name__ == "__main__":
    arg_parser = createArgumentParser()
    args = arg_parser.parse_args()

    if args.type == 'N':
        processDotNet(args.rootdir, args.version, args.build_type, args.build_no)

