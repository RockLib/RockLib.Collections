# RockLib.Collections Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## 3.0.0 - 2024-01-04

#### Changed

- Removed netcoreapp3.1 and added net8.0.

## 2.0.0 - 2022-01-25

#### Added

- Added `.editorconfig` and `Directory.Build.props` files to ensure consistency.

#### Changed

- Supported targets: net6.0, netcoreapp3.1, and net48.
- As the package now uses nullable reference types, some method parameters now specify if they can accept nullable values.

## 1.0.6 - 2021-08-11

#### Changed

- Changes "Quicken Loans" to "Rocket Mortgage".

## 1.0.5 - 2021-05-06

#### Added

- Adds SourceLink to nuget package.

----

**Note:** Release notes in the above format are not available for earlier versions of
RockLib.Collections. What follows below are the original release notes.

----

## 1.0.4

Adds net5.0 target.

## 1.0.3

Adds icon to project and nuget package.

## 1.0.2

Updates to align with nuget conventions.

## 1.0.1

Embeds license in the nuget package.

## 1.0.0

Initial release, contains NamedCollection<T> class.
