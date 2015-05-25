#!/usr/bin/env python

import argparse
import os

version = 'v2.0.0'
ignored = {
	'Thumbs.db'
}

def walkFiles(inDir):
	for path, dirnames, filenames in os.walk(inDir):
		for filename in filenames:
			if filename not in ignored:
				yield os.path.join(path, filename)

def main():
	parser = argparse.ArgumentParser(prog='savepics',
		description='Sorts picture files from an input directory into an output directory, using a folder structure with the pattern YYYY/YYYY.MM.'
	)
	parser.add_argument('-v', '--version', action='version', version='%(prog)s {}'.format(version))
	parser.add_argument('-n', '--dry-run', action='store_true',
		help='Prints out what would be written without actually writing.'
	)
	parser.add_argument('input', action='store',
		help='Directory to read and sort.'
	)
	parser.add_argument('output', action='store',
		help='Directory to write sorted pictures to.'
	)
	args = parser.parse_args()
	
	for filename in walkFiles(args.input):
		print(filename)

if __name__ == '__main__':
	main()